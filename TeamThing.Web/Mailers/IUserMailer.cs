using System;
using System.Linq;
using System.Net.Mail;
using TeamThing.Model;

namespace TeamThing.Web.Mailers
{ 
    public interface IUserMailer
    {

        MailMessage ThingAssigned(User[] assignedTo, Thing thing);
        MailMessage ThingUnassigned(User[] unAssignedTo, Thing thing);


        MailMessage ApprovedForTeam(User user, Team team);


        MailMessage DeniedTeam(User user, Team team);


        MailMessage InvitedToTeam(User sendTo, User inviter, Team team);


        MailMessage ThingCompleted(User[] assignedTo, User completer, Thing thing);
		
		
	}
}