using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;

namespace TeamThing.Web.Core
{
    public static class MailExtensions
    {
        public static void Send(this System.Net.Mail.MailMessage message)
        {
            if (message != null)
            {
                message.Send();
            }
        }
    }
}