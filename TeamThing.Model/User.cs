using System;
using System.Collections.Generic;

namespace TeamThing.Model
{
    public class User
    {
        public User(string emailAddress)
        {
            this.EmailAddress = emailAddress;
            this.DateCreated = DateTime.Now;
        }

        private User()
        {
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