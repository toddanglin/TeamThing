using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.OpenAccess;

namespace Mercury.Model
{
    [Serializable]
    [Telerik.OpenAccess.Persistent(IdentityField = "teamMemberId")]
    public class TeamMember
    {
        private string firstName;
        private string lastName;
        private string email;
        private string profileColor;
        private int teamMemberId;
        private IList<ThingBase> things;
        private Team team;
        private Guid apiKey;

        public TeamMember()
        {
            apiKey = new Guid();
        }

        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                firstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                lastName = value;
            }
        }

        [FieldAlias("email")]
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
            }
        }

        public string ProfileColor
        {
            get
            {
                return profileColor;
            }
            set
            {
                profileColor = value;
            }
        }

        [FieldAlias("teamMemberId")]
        public int TeamMemberId
        {
            get
            {
                return teamMemberId;
            }
            set
            {
                teamMemberId = value;
            }
        }

        [FieldAlias("team")]
        public Team Team
        {
            get { return team; }
        }

        [FieldAlias("things")]
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

        [FieldAlias("apiKey")]
        public Guid ApiKey
        {
            get { return apiKey; }
            set { apiKey = value; }
        }
    }
}
