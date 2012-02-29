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
    public class UndoThingProcessor : ThingProcessorBase
    {
        public UndoThingProcessor()
        {

        }

        public override IEmail Process(string thingData, Model.TeamMember teamMember, Message message, Telerik.OpenAccess.IObjectScope scope)
        {
            thingData = thingData.Substring(thingData.IndexOf('#', 1)+1);//Get all text after close hashtag

            //TODO
            //1) Try to find original item (must be from this TeamMember, Today, with same Description)
            //2) Change status to done

            var tasks = new TaskRepository(scope);
            var updatedTask = tasks.ChangeStatus(teamMember, thingData, TaskStatus.InProgress);

            return CreateEmail<EmailDoNotSend>(teamMember, updatedTask);
        }

    }
}
