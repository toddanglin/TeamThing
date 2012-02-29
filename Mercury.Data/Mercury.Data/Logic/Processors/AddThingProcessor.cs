using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mercury.Model;
using System.Net.Mail;
using ImapX;

namespace Mercury.Data.Logic
{
    /// <summary>
    /// Adds thing to today's tasks for specified team member
    /// </summary>
    public class AddThingProcessor : ThingProcessorBase
    {
        public AddThingProcessor()
        {

        }

        public override IEmail Process(string thingData, Model.TeamMember teamMember, Message message, Telerik.OpenAccess.IObjectScope scope)
        {
            thingData = thingData.Trim();
            string keyMetaData = string.Empty; //Used for passing extra commands as part of ADD

            if (thingData.StartsWith("#"))
            {
                int indexOfSecondHash = thingData.IndexOf('#', 1);
                keyMetaData = thingData.Substring(1, indexOfSecondHash - 1).ToLower(); //Data between the hashtags
                thingData = thingData.Substring(indexOfSecondHash + 1);//Get all text after close hashtag                
            }

            //Check for meta data in the hashtags
            //With the ADD operation, users are allowed to add "in the future" by 
            //specifying a number of days in the future to add the task from today
            //EX: #ADD4# = Add task with date 4 days in the future
            //    #ADD 4# = Add task with date 4 days in the future (spaces don't matter)

            //To check for metadata, first we need to remove keyword
            keyMetaData = Regex.Replace(keyMetaData, "add|\\s*", String.Empty, RegexOptions.IgnoreCase);
            int daysDelay = 0;
            if (!String.IsNullOrWhiteSpace(keyMetaData))
            {
                //Key metadata exists. Only expected option is # number of days to delay
                //Try to get number of days               
                int.TryParse(keyMetaData, out daysDelay);
            }

            //Check for additional markers in text
            IThing thing = null;
            if (Regex.Match(thingData, "#blocking|#fail", RegexOptions.IgnoreCase).Success)
            {
                //This is an obstacle
                var or = new ObstacleRepository(scope);

                var o = new Obstacle()
                {
                    DateCreated = message.Date,
                    Description = thingData.Trim(),
                    TeamMember = teamMember
                };

                or.Add(o);
                thing = o;
            }
            else
            {
                var tr = new TaskRepository(scope);

                var t = new Task()
                {
                    DateCreated = message.Date.AddDays(daysDelay),
                    Description = thingData.Trim(),
                    TeamMember = teamMember,
                    Status = TaskStatus.InProgress
                };

                if (tr.IsDuplicate(t))
                    return null;
                    
                tr.Add(t);
                thing = t;
            }

            return CreateEmail<EmailTaskAddSuccess>(teamMember, thing);
        }
    }
}
