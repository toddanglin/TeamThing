using System;
using System.Collections.Generic;

namespace TeamThing.Model
{
    public class User
    {
        public User(string emailAddress)
            : this()
        {
            this.EmailAddress = emailAddress;
            this.DateCreated = DateTime.Now;
        }

        private User()
        {
            this.Things = new List<UserThing>();
            this.Teams = new List<TeamUser>();
        }

        public string ImagePath { get; set; }
        public int Id { get; private set; }
        public string EmailAddress { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateCreated { get; private set; }
        public bool IsActive { get; set; }
        public IList<UserThing> Things { get; private set; }
        public IList<TeamUser> Teams { get; private set; }
        public int Color { get; set; }
    }
}