using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Model
{
    [Telerik.OpenAccess.Persistent()]
    public interface IThing
    {
        int ThingId
        {
            get;
            set;
        }

        string Description
        {
            get;
            set;
        }

        TeamMember TeamMember
        {
            get;
            set;
        }

        DateTime DateCreated
        {
            get;
            set;
        }

        ThingSource ThingSource
        {
            get;
            set;
        }
    }
}
