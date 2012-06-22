using System;
using System.Linq;
using System.Net.Mail;
using Mvc.Mailer;
using TeamThing.Model;

namespace TeamThing.Web.Mailers
{
    public class UserMailer : MailerBase, IUserMailer
    {
        public UserMailer() :
            base()
        {
            MasterName = "_Layout";
        }

        public virtual MailMessage ThingAssigned(User[] assignedTo, Thing thing, User assignedBy)
        {
            var mailMessage = new MailMessage { Subject = "TeamThing - New Thing to Do!" };


            var sendTo = assignedTo.Select(u => u.EmailAddress);

            if (sendTo.Count() > 0)
            {
                mailMessage.To.Add(string.Join(",", sendTo));
                mailMessage.From = new MailAddress(fromAddress);
                //ViewBag.Data = someObject;
                ViewData.Model = new ThingChanged { Thing = thing, ChangeMadeBy = assignedBy };
                PopulateBody(mailMessage, viewName: "ThingAssigned");

                return mailMessage;
            }

            return null;
        }
        public virtual MailMessage ThingUnassigned(User[] unassignedTo, Thing thing, User remover)
        {
            var mailMessage = new MailMessage { Subject = "TeamThing - Something off your plate!" };

            var sendTo = unassignedTo.Select(u => u.EmailAddress);

            if (sendTo.Count() > 0)
            {
                mailMessage.To.Add(string.Join(",", sendTo));
                mailMessage.From = new MailAddress(fromAddress);
                //ViewBag.Data = someObject;
                ViewData.Model = new ThingChanged { Thing = thing, ChangeMadeBy = remover };
                PopulateBody(mailMessage, viewName: "ThingUnassigned");

                return mailMessage;
            }

            return null;
        }

        public virtual MailMessage ApprovedForTeam(User user, Team team)
        {
            var mailMessage = new MailMessage { Subject = "TeamThing - Team Access Approved!" };

            mailMessage.To.Add(user.EmailAddress);
            mailMessage.From = new MailAddress(fromAddress);
            //ViewBag.Data = someObject;

            ViewData.Model = new TeamAccessChanged { Team = team};
            PopulateBody(mailMessage, viewName: "ApprovedForTeam");

            return mailMessage;
        }

        public virtual MailMessage DeniedTeam(User user, Team team)
        {
            var mailMessage = new MailMessage { Subject = "TeamThing - Team Access Denied" };

            mailMessage.To.Add(user.EmailAddress);
            mailMessage.From = new MailAddress(fromAddress);
            //ViewBag.Data = someObject;

            ViewData.Model = new TeamAccessChanged { Team = team };
            PopulateBody(mailMessage, viewName: "DeniedTeam");

            return mailMessage;
        }

        public virtual MailMessage InvitedToTeam(User sendTo, User inviter, Team team)
        {
            var mailMessage = new MailMessage { Subject = "TeamThing - Invited to Join Team" };

            mailMessage.To.Add(sendTo.EmailAddress);
            mailMessage.From = new MailAddress(fromAddress);
            //ViewBag.Data = someObject;

            ViewData.Model = new TeamAccessChanged { Team = team };
            PopulateBody(mailMessage, viewName: "InvitedToTeam");

            return mailMessage;
        }

        private const string fromAddress = "no-reply@teamthing.net";

        public virtual MailMessage ThingCompleted(User[] assignedTo, User completer, Thing thing)
        {
            var sendTo = assignedTo.Where(u => u.Id != completer.Id).Select(u => u.EmailAddress);

            if (sendTo.Count() > 0)
            {
                var mailMessage = new MailMessage { Subject = "TeamThing - Thing Completed" };
                mailMessage.To.Add(string.Join(",", sendTo));
                mailMessage.From = new MailAddress(fromAddress);
                //ViewBag.Data = someObject;
                ViewData.Model = new ThingChanged { Thing = thing, ChangeMadeBy = completer };
                PopulateBody(mailMessage, viewName: "ThingCompleted");

                return mailMessage;
            }

            return null;
        }
    }
  
    public class TeamAccessChanged : object
    {
        public Team Team { get; set; }
    }
  
    public class ThingChanged : object
    {
        public Thing Thing { get; set; }

        public User ChangeMadeBy { get; set; }
    }
}