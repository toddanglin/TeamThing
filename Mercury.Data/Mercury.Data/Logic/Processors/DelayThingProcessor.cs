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
    /// Marks existing things today as done
    /// </summary>
    public class DelayThingProcessor : ThingProcessorBase
    {
        public DelayThingProcessor()
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
            //With the DELAY operation, users are allowed to delay extra days by 
            //specifying a number of days in the future to add the task from today
            //EX: #DELAY4# = Delays task by 4 days
            //    #DELAY 4# = Delays task by 4 days (spaces don't matter)

            //To check for metadata, first we need to remove keyword
            keyMetaData = Regex.Replace(keyMetaData, "delay|\\s*", String.Empty, RegexOptions.IgnoreCase);
            int daysDelay = 1;
            if (!String.IsNullOrWhiteSpace(keyMetaData))
            {
                //Key metadata exists. Only expected option is # number of days to delay
                //Try to get number of days               
                int.TryParse(keyMetaData, out daysDelay);
            }

            //TODO
            //1) Try to find original item (must be from this TeamMember, Today, with same Description)
            //2) Change status to done

            var tasks = new TaskRepository(scope);
            var updatedTask = tasks.Delay(teamMember, thingData, daysDelay);

            if (updatedTask == null) //Error occured
                return CreateEmail<EmailDelaySuccess>(teamMember, updatedTask);
            else
                return CreateEmail<EmailDelayError>(teamMember, updatedTask);
        }
    }
}
