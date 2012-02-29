using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Mercury.Data.Util;
using Telerik.OpenAccess;
using Mercury.Model;
using System.Diagnostics;

namespace Mercury.Data.Logic
{
    public static class Emailer
    {
        public static bool IsEmailSending = false;

        public static IEmail CreateEmail<T, C>(TeamMember tm, IThing o)
            where T : IEmail, new()
            where C : ITemplateContext, new()
        {
            IEmail email = null;
            email = new T();

            var context = new C()
            {
                FirstName = tm.FirstName,
                TeamMember = tm,
                ToAddress = tm.Email,
            };

            if (o != null)
                context.Things.Add(o);

            email.Context = context;

            return email;
        }

        public static IEmail CreateEmail<T>(TeamMember tm, IThing o)
            where T : IEmail, new()
        {
            return CreateEmail<T, DefaultTemplateContext>(tm, o);
        }

        public static void Send(IEmail email, IObjectScope scope)
        {
            var body = GetMergedTemplate<ITemplateContext>(email.Template, email.Context);

            var msg = new MailMessage("teamthing@gmail.com", email.Context.ToAddress, email.Subject, body);
            msg.From = new MailAddress("teamthing@gmail.com", "TeamThing");
            msg.IsBodyHtml = true;

            var smtp = new SmtpClient();
            smtp.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);

            var state = new Mercury.Data.Logic.AysncStateHelper()
            {
                Scope = scope,
                Context = email.Context,
                Message = msg,
                Email = email
            };

            Trace.Write(String.Format("Preparing to send msg \"{0}\" to \"{1}\"", msg.Subject, email.Context.ToAddress));
            smtp.SendAsync(msg, state);
            IsEmailSending = true;
        }

        private static void smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Trace.Write(String.Format("Error sending email: {0}", e.Error.Message));
                return;
            }
            Trace.Write("Email message successfully sent.");


            //When an email is sent, log it in the database
            var state = e.UserState as Mercury.Data.Logic.AysncStateHelper;

            if (state.Scope == null || state.Context == null || state.Email == null || state.Message == null)
                return;

            try
            {
                //Save email log history
                var log = new EmailLog()
                {
                    TeamMember = state.Context.TeamMember,
                    ToAddress = state.Message.To[0].Address,
                    Subject = state.Message.Subject,
                    SendDate = DateTime.Now,
                    Team = state.Context.TeamMember.Team,
                    EmailType = state.Email.EmailType
                };
                state.Scope.Add(log);
                state.Scope.Transaction.Commit();

                Trace.Write(String.Format("Email log for \"{0}\" sent to \"{1}\" saved in DB.", state.Message.Subject, state.Context.ToAddress));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(String.Format("Error trying to save email log: {0} {1}", ex.Message, (ex.InnerException == null) ? String.Empty : ex.InnerException.Message));
            }

            IsEmailSending = false;
        }

        public static int SendStartOfDayReminder(int teamId, IObjectScope scope)
        {
            //TODO: 
            //1) Get all team members for current team
            //   WHERE no tasks added for current day
            //2) Send email to each team member that meets criteria

            int emailCount = 0;
            var teamRepository = new TeamRepository(scope);

            var tm = teamRepository.GetMembersWithNoCurrentTasks(teamId);
            if (tm == null || tm.Count <= 0)
            {
                Trace.WriteLine("No start of day reminders sent. No team members found.");
                return 0;
            }

            foreach (var t in tm)
            {
                var email = CreateEmail<EmailReminderStartDay>(t, null);
                Send(email, scope);

                emailCount++;
            }

            return emailCount;
        }

        public static int SendEndOfDayReminder(int teamId, IObjectScope scope)
        {
            //TODO: 
            //1) Get all team members for current team
            //   WHERE no tasks added for current day
            //2) Send email to each team member that meets criteria
            
            int emailCount = 0;
            var teamRepository = new TeamRepository(scope);

            var tm = teamRepository.GetMembersCurrentTasks(teamId);
            if (tm == null || tm.Count <= 0)
            {
                Trace.WriteLine("No end of day reminders sent. No team members found.");
                return 0;
            }

            var taskRepository = new TaskRepository(scope);
            foreach (var t in tm)
            {
                var openThings = taskRepository.GetTodayTasksByPerson(t);
                var email = CreateEmail<EmailReminderEndDay>(t, null);
                email.Context.Things = openThings.Cast<IThing>().ToList();

                Send(email, scope);

                emailCount++;
            }

            return emailCount;
        }

        public static string GetMergedTemplate<T>(string template, T contextInstance)
        {
            if (String.IsNullOrWhiteSpace(template))
                return null;

            return contextInstance.ToStringWithFormat(template);
        }    
        
    }

    public class AysncStateHelper
    {
        public IObjectScope Scope { get; set; }
        public ITemplateContext Context { get; set; }
        public MailMessage Message { get; set; }
        public IEmail Email { get; set; }
    }
}
