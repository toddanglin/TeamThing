using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Model;

namespace Mercury.Data.Logic
{
    public class EmailUnknownCommand : EmailBase
    {
        public EmailUnknownCommand() : base("Confused By Your Last Request", "EmailTemplates.TaskUnknown.html", EmailType.ErrorInformation) { }
    }

    public class EmailHelpCommand : EmailBase
    {
        public EmailHelpCommand() : base("TeamThing Help", "EmailTemplates.KeywordHelp.html", EmailType.General) { }
    }

    public class EmailDoNotSend : EmailBase
    {
        public EmailDoNotSend() : base("DUMMY TEMPLATE", "", EmailType.General) { }
    }

    public class EmailTaskAddSuccess : EmailBase
    {
        public EmailTaskAddSuccess() : base("Things Successfully Added", "EmailTemplates.TaskAddSuccess.html", EmailType.SuccessInformation) { }
    }

    public class EmailTaskDone : EmailBase
    {
        public EmailTaskDone() : base("Things Successfully Marked Done", "EmailTemplates.TaskDone.html", EmailType.SuccessInformation) { }
    }

    public class EmailTaskDoneError : EmailBase
    {
        public EmailTaskDoneError() : base("Problem Marking Things Done", "EmailTemplates.TaskDoneError.html", EmailType.ErrorInformation) { }
    }

    public class EmailTaskRemoved : EmailBase
    {
        public EmailTaskRemoved() : base("Things Removed Successfully", "EmailTemplates.TaskRemoved.html", EmailType.SuccessInformation) { }
    }

    public class EmailTaskRemoveError : EmailBase
    {
        public EmailTaskRemoveError() : base("Trouble Removing Things", "EmailTemplates.TaskRemoveError.html", EmailType.ErrorInformation) { }
    }

    public class EmailOpenTaskList : EmailBase
    {
        public EmailOpenTaskList() : base("Your Open Things for Today", "EmailTemplates.OpenTaskList.html", EmailType.General) { }
    }

    public class EmailDelaySuccess : EmailBase
    {
        public EmailDelaySuccess() : base("Things Delayed Successfully", "EmailTemplates.TaskDelay.html", EmailType.SuccessInformation) { }
    }

    public class EmailDelayError : EmailBase
    {
        public EmailDelayError() : base("Things Could Not Be Delayed", "EmailTemplates.TaskDelayError.html", EmailType.ErrorInformation) { }
    }

    public class EmailReminderStartDay : EmailBase
    {
        public EmailReminderStartDay() : base("Remember to Add Things Today", "EmailTemplates.ReminderStartDay.html", EmailType.StartOfDayReminder) { }
    }

    public class EmailReminderEndDay : EmailBase
    {
        public EmailReminderEndDay() : base("Remember to Mark Things Done Today", "EmailTemplates.ReminderEndDay.html", EmailType.EndOfDayReminder) { }
    }

    #region API KEY EMAIL TEMPLATES
    public class ApiKeySuccess : EmailBase
    {
        public ApiKeySuccess() : base("Your TeamThing API Key", "EmailTemplates.ApiKeySuccess.html", EmailType.General) 
        {
        }
    }

    public class ApiKeyError: EmailBase
    {
        public ApiKeyError() : base("Problem with Your TeamThing API Key", "EmailTemplates.ApiKeyError.html", EmailType.General) 
        {
        }
    }
    #endregion
}
