using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;

namespace TeamThing.Web.Mailers
{ 
    public class UserMailer : MailerBase, IUserMailer     
	{
		public UserMailer():
			base()
		{
			MasterName="_Layout";
		}

		
		public virtual MailMessage ThingAssigned()
		{
			var mailMessage = new MailMessage{Subject = "ThingAssigned"};
			
			mailMessage.To.Add("jholt456@gmail.com");
			mailMessage.From = new MailAddress("jholt456@gmail.com");
			//ViewBag.Data = someObject;
			PopulateBody(mailMessage, viewName: "ThingAssigned");

			return mailMessage;
		}

		
		public virtual MailMessage ApprovedForTeam()
		{
			var mailMessage = new MailMessage{Subject = "ApprovedForTeam"};

            mailMessage.To.Add("jholt456@gmail.com");
            mailMessage.From = new MailAddress("jholt456@gmail.com");
			//ViewBag.Data = someObject;
			PopulateBody(mailMessage, viewName: "ApprovedForTeam");

			return mailMessage;
		}

		
		public virtual MailMessage DeniedTeam()
		{
			var mailMessage = new MailMessage{Subject = "DeniedTeam"};

            mailMessage.To.Add("jholt456@gmail.com");
            mailMessage.From = new MailAddress("jholt456@gmail.com");
			//ViewBag.Data = someObject;
			PopulateBody(mailMessage, viewName: "DeniedTeam");

			return mailMessage;
		}

		
		public virtual MailMessage InvitedToTeam()
		{
			var mailMessage = new MailMessage{Subject = "InvitedToTeam"};

            mailMessage.To.Add("jholt456@gmail.com");
            mailMessage.From = new MailAddress("jholt456@gmail.com");
			//ViewBag.Data = someObject;
			PopulateBody(mailMessage, viewName: "InvitedToTeam");

			return mailMessage;
		}

		
		public virtual MailMessage ThingCompleted()
		{
			var mailMessage = new MailMessage{Subject = "ThingCompleted"};

            mailMessage.To.Add("jholt456@gmail.com");
            mailMessage.From = new MailAddress("jholt456@gmail.com");
			//ViewBag.Data = someObject;
			PopulateBody(mailMessage, viewName: "ThingCompleted");

			return mailMessage;
		}

		
	}
}