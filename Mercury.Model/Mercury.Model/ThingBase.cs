using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.OpenAccess;

namespace Mercury.Model
{
    [Serializable]
    [Telerik.OpenAccess.Persistent()]
    public abstract class ThingBase : IThing
    {
        protected DateTime dateCreated;
        protected string description;
        protected TeamMember teamMember;
        protected int thingId;
        protected ThingSource thingSource;

        #region IThing Members

        [FieldAlias("thingId")]
        public int ThingId
        {
            get
            {
                return thingId;
            }
            set
            {
                thingId = value;
            }
        }

        [FieldAlias("dateCreated")]
        public DateTime DateCreated
        {
            get
            {
                return dateCreated;
            }
            set
            {
                dateCreated = value;
            }
        }

        [FieldAlias("description")]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        [FieldAlias("teamMember")]
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
        #endregion

        [FieldAlias("thingSource")]
        public ThingSource ThingSource
        {
            get
            {
                return thingSource;
            }
            set
            {
                thingSource = value;
            }
        }
    }
}
