using System;
using System.Collections.Generic;
using Mercury.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.OpenAccess;

namespace Mercury.Model
{
    [Serializable]
    [Telerik.OpenAccess.Persistent(IdentityField = "teamId")]
    public class Team
    {
        private int teamId;
        private IList<TeamMember> teamMembers;
        private string teamName;
        private bool isEnabled;

        [FieldAlias("teamId")]
        public int TeamId
        {
            get
            {
                return teamId;
            }
            set
            {
                teamId = value;
            }
        }

        [FieldAlias("teamMembers")]
        public IList<TeamMember> TeamMembers
        {
            get
            {
                return teamMembers;
            }
            set
            {
                teamMembers = value;
            }
        }

        [FieldAlias("teamName")]
        public string TeamName
        {
            get
            {
                return teamName;
            }
            set
            {
                teamName = value;
            }
        }

        [FieldAlias("isEnabled")]
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
            }
        }
    }
}
