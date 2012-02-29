using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Model;
using ImapX;
using System.Text.RegularExpressions;
using Telerik.OpenAccess;
using Mercury.Data.Logic;
using Mercury.Data.Util;
using System.Diagnostics;
using System.Configuration;

namespace Mercury.Data
{
    /// <summary>
    /// Responsible for getting tasks from email and parsign in to tasks
    /// </summary>
    public class TaskCollector
    {      
        private readonly string emailUsername;
        private readonly string emailPassword;
        private readonly string emailServer;
        private readonly int emailPort;
        private IObjectScope scope;

        public TaskCollector(string username, string password, string server, int port, IObjectScope dbScope)
        {
            emailUsername = username;
            emailPassword = password;
            emailServer = server;
            emailPort = port;
            scope = dbScope;
        }

        public void ParseTasks(List<TeamMember> teamMembers)
        {
            //TODO:
            //1) Get emails from team members
            //2) Parse Thing type (task/obstacle)
            //3) Parse tasks from email (one per line of body)
            //4) Save tasks to database

            //Folders to Get/Move messages to/from
            bool isTest = (ConfigurationManager.AppSettings["testMode"] == null) ? false : Convert.ToBoolean(ConfigurationManager.AppSettings["testMode"]);
            Trace.WriteLine(String.Format("Test Mode status: {0}", isTest));
            string INBOX = (isTest) ? "TESTBOX" : "INBOX";
            string PROCESSSED = (isTest) ? "TESTPROCESSED" : "PROCESSED";

            ImapClient client = CreateImapConnection();
            ImapX.MessageCollection messages = client.Folders[INBOX].Search("ALL", true);

            if (messages.Count() <= 0)
            {
                Trace.WriteLine("No messages found in INBOX. Stopping process.");
                return;
            }

            Trace.Write("Message count greater than zero.");

            //Filter messages to only messages for current team
            List<String> teamEmail = teamMembers.Select(t => t.Email.ToLower()).ToList();

            var filteredMsgs = messages.Where(m => teamEmail.Contains(m.From[0].Address.ToLower())).Select(m => m).ToList();

            if (filteredMsgs.Count() <= 0)
            {
                Trace.WriteLine("No filtered messages found. Stopping processing.");
                return;
            }

            Trace.Write("Filtered message count greater than zero.");

            //With filtered messages, try to process each and save (based on type)
            foreach (var m in filteredMsgs)
            {
                var tm = teamMembers.Where(t => t.Email.ToLower() == m.From[0].Address.ToLower()).FirstOrDefault();
                Trace.Write(String.Format("Processing message for Team Member {0}", tm.FirstName));
                ProcessMessage(m, tm);

                //Move message out of inbox to avoid repeat processing
//#if !DEBUG
                client.Folders[INBOX].MoveMessageToFolder(m, client.Folders[PROCESSSED]);
//#endif
            }

            //Process email queue
            foreach (var item in emailQueue)
            {
                Emailer.Send(item.Value, scope);
            }
        }

        private Dictionary<KeyValuePair<string, string>, IEmail> emailQueue = new Dictionary<KeyValuePair<string, string>, IEmail>();

        private void ProcessMessage(Message m, TeamMember tm)
        {
            //TODO:
            //1) Check archive to make sure this is not a duplicate message
            //2) Parse out details and save new tasks (one per line break)     
            //3) Save original message to DB for archiving

            if(CheckForDupeEmail(m))
                return; //Email has already been processed. Don't do it again.

            //NEW! There are some "shortcuts" that process based on pre-defind subjects
            //Check for those first b/c if found, no additional processing required
            //(TODO: Refactor to better pattern)
            if (m.Subject != null)
            {
                var subject = m.Subject.Trim().ToLower();
                if (subject == "open")
                {
                    //Send email with all open items for current team member                    
                    var tr = new TaskRepository(scope);

                    var things = tr.GetTodayTasksByPerson(tm).Cast<IThing>().ToList();
                    var email = Emailer.CreateEmail<EmailOpenTaskList>(tm, null);
                    email.Context.Things = things;

                    AddToEmailQueue(email);

                    return;
                }
                else if (subject == "help")
                {
                    var email = Emailer.CreateEmail<EmailHelpCommand>(tm, null);
                    AddToEmailQueue(email);

                    return;
                }
                else if (subject == "api")
                {
                    var teamRepository = new TeamRepository(scope);

                    Guid key;
                    if (tm.ApiKey == null || tm.ApiKey == Guid.Empty)
                        key = teamRepository.GenerateNewApiKey(tm.TeamMemberId);
                    else
                        key = tm.ApiKey;

                    IEmail email;
                    if (key == Guid.Empty)
                        email = Emailer.CreateEmail<ApiKeyError, ApiKeyTemplateContext>(tm, null);
                    else
                        email = Emailer.CreateEmail<ApiKeySuccess, ApiKeyTemplateContext>(tm, null);

                    //TODO: Add more security to key process 
                    var context = email.Context as ApiKeyTemplateContext;
                    context.ApiKey = key;

                    AddToEmailQueue(email);

                    return;
                }
            }
            

            //Get body contents
            //IF TEXT - Clean "HTML simulation" charcters (such as "*   " for lists)
            //IF HTML - Remove HTML tags (prefer HTML since most emails come in this format)
            string body = (!String.IsNullOrWhiteSpace(m.TextBody.TextData)) ? m.TextBody.TextData.Replace("*   ",String.Empty): m.HtmlBody.TextData.ToPlainText();
            //FIX - For some reason, random =\r\n sequences show-up in email body. Needs to be cleaned.
            body = body.Replace("=\r\n", String.Empty);

            if (String.IsNullOrWhiteSpace(body)) //No tasks included in email - ignore
            {
                return;
            }

            List<String> taskData = body.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<IThing> foundThings = new List<IThing>();

            //It's a reply - treat contents differently (new emails assumed to have "clean" body - only adds)
            //In a reply only look for lines that start with keywords
            foreach (var t in taskData)
            {
                string keyword = String.Empty;
                //Match lines that start with #SOMETHING#
                var match = Regex.Match(t.Trim(), @"^#[^\s]+#", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    //We've found a line that we care about - process directive
                    keyword = match.Value.Replace("#", String.Empty);
                }

                //If new "non-reply" email, also accept items with no keyword
                //(assumed that fresh emails only have "clean" content
                if(keyword == string.Empty && (m.Subject == null || !m.Subject.ToLower().StartsWith("re:")))
                    keyword = "ADD"; //New, "FRESH" email - all lines considered ADD actions

                if (keyword == String.Empty)
                    continue; //No keywords found and not new email

                IThingProcessor processor = ThingProcessorFactory.GetProcessor(keyword);
                if (processor == null)
                {
                    //Unrecognized keyword. Send an error email.
                    var errEmail = new EmailUnknownCommand();
                    var context = new DefaultTemplateContext()
                    {
                        FirstName = tm.FirstName,
                        TeamMember = tm,
                        ToAddress = tm.Email,
                        Things = null
                    };
                    errEmail.Context = context;
                    AddToEmailQueue(errEmail);
                }
                else
                {
                    var email = processor.Process(t.Trim(), tm, m, scope);

                    //Get things for archival purposes
                    if(email != null){
                        foundThings.AddRange(email.Context.Things);
                        AddToEmailQueue(email);
                    }
                }
            }

            //Archive original message
            var archiveThings = foundThings.Cast<ThingBase>().ToList();
            ArchiveInboundMessage(tm, body, m, archiveThings);
        }

        private bool CheckForDupeEmail(Message msg)
        {
            var dupe = scope.Extent<ThingSource>().Where(t => t.MessageDate == msg.Date
                            && t.FromAddress == msg.From[0].Address
                            && t.MessageId == msg.MessageId).FirstOrDefault();

            if (dupe == null) //No match found, not a dupe
                return false;

            return true; //match found, email already processed
        }
        
        /// <summary>
        /// Adds email to queue to be sent at end of p
        /// </summary>
        /// <param name="email"></param>
        private void AddToEmailQueue(IEmail email)
        {
            if (typeof(EmailDoNotSend) == email.GetType()) //Don't send emails using this template
                return;

            var key = new KeyValuePair<string,string>(email.Context.ToAddress, email.ResourcePath);
            var original = (emailQueue.ContainsKey(key)) ? emailQueue[key] : null;
            if (original == null)
            {
                //No emails yet for this person with this template. Create a new one to add to queue.
                emailQueue.Add(key, email);
            }
            else
            {
                //This tempalte is already going to be sent to user
                //Merge the "Things" in context
                original.Context.Things.AddRange(email.Context.Things);
                emailQueue[key] = original;
            }
        }

        private void ArchiveInboundMessage(TeamMember tm, string body, Message m, List<ThingBase> newThings)
        {
            var archive = new ThingSource()
            {
                TeamMember = tm,
                MessageBody = body,
                MessageDate = m.Date,
                MessageId = m.MessageId,
                DateImported = DateTime.Now,
                FromAddress = m.From[0].Address,
                Things = newThings
            };

            scope.Add(archive);
        }

        private ImapClient CreateImapConnection()
        {
            ImapX.ImapClient client = new ImapX.ImapClient(emailServer, emailPort, true);

            if (!client.Connection())
                throw new ApplicationException(String.Format("Failed to connect to email server {0}", emailServer));

            if (!client.LogIn(emailUsername, emailPassword))
                throw new ApplicationException(String.Format("Failed to log-in to email server with {0}", emailUsername));

            return client;
        }
    }
}
