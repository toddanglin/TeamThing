using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.OpenAccess;

namespace Mercury.Model
{
    [Serializable]
    [Telerik.OpenAccess.Persistent(IdentityField = "thingSourceId")]
    public class ThingSource
    {
        private IList<ThingBase> things;
        private int thingSourceId;
        private string messageId;
        private string fromAddress;
        private DateTime messageDate;
        private string messageBody;
        private DateTime dateImported;
        private TeamMember teamMember;

        [FieldAlias("thingSourceId")]
        public int ThingSourceId
        {
            get
            {
                return thingSourceId;
            }
            set
            {
                thingSourceId = value;
            }
        }

        [FieldAlias("messageId")]
        public string MessageId
        {
            get
            {
                return messageId;
            }
            set
            {
                messageId = value;
            }
        }

        [FieldAlias("fromAddress")]
        public string FromAddress
        {
            get
            {
                return fromAddress;
            }
            set
            {
                fromAddress = value;
            }
        }

        [FieldAlias("messageDate")]
        public DateTime MessageDate
        {
            get
            {
                return messageDate;
            }
            set
            {
                messageDate = value;
            }
        }

        [FieldAlias("messageBody")]
        public string MessageBody
        {
            get
            {
                return messageBody;
            }
            set
            {
                messageBody = value;
            }
        }

        [FieldAlias("dateImported")]
        public DateTime DateImported
        {
            get
            {
                return dateImported;
            }
            set
            {
                dateImported = value;
            }
        }

        public IList<ThingBase> Things
        {
            get
            {
                return things;
            }
            set
            {
                things = value;
            }
        }

        public TeamMember TeamMember
        {
            get { return teamMember; }
            set { teamMember = value; }
        }

    }
}
