using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;

namespace TeamThing.Web.Mailers
{ 
    public interface IUserMailer
    {
				
		MailMessage ThingAssigned();
		
				
		MailMessage ApprovedForTeam();
		
				
		MailMessage DeniedTeam();
		
				
		MailMessage InvitedToTeam();
		
				
		MailMessage ThingCompleted();
		
		
	}
}