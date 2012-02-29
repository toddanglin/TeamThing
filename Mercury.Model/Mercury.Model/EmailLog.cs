using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Model
{
    [Serializable]
    [Telerik.OpenAccess.Persistent(IdentityField = "emailLogId")]
    public class EmailLog
    {
        private int emailLogId;
        private string subject;
        private string toAddress;
        private DateTime sendDate;        
        private Team team;
        private TeamMember teamMember;        
        private EmailType emailType;

        public int EmailLogId
        {
            get
            {
                return emailLogId;
            }
            set
            {
                emailLogId = value;
            }
        }

        public EmailType EmailType
        {
            get
            {
                return emailType;
            }
            set
            {
                emailType = value;
            }
        }

        public DateTime SendDate
        {
            get
            {
                return sendDate;
            }
            set
            {
                sendDate = value;
            }
        }

        public string Subject
        {
            get
            {
                return subject;
            }
            set
            {
                subject = value;
            }
        }

        public Team Team
        {
            get
            {
                return team;
            }
            set
            {
                team = value;
            }
        }

        public TeamMember TeamMember
        {
            get
            {
                return teamMember;
            }
            set
            {
                teamMember = value;
            }
        }

        public string ToAddress
        {
            get
            {
                return toAddress;
            }
            set
            {
                toAddress = value;
            }
        }
    }
}
