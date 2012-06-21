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

        public User(string oAuthProvider, string oAuthUserId)
            : this()
        {
            this.OAuthProvider = oAuthProvider;
            this.OAuthUserId = oAuthUserId;
            this.DateCreated = DateTime.Now;
        }

        private User()
        {
            this.Things = new List<UserThing>();
            this.Teams = new List<TeamUser>();
            this.ThingLog = new List<ThingLog>();
            this.StarredThings = new List<Thing>();
        }

        public string OAuthProvider { get; set; }
        public string OAuthUserId { get; set; }

        public string ImagePath { get; set; }
        //public string PasswordHash { get; set; }
        //public string PasswordSalt { get; set; }
        public int Id { get; private set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateCreated { get; private set; }
        public bool IsActive { get; set; }
        public IList<UserThing> Things { get; private set; }
        public IList<Thing> StarredThings { get; private set; }
        public IList<ThingLog> ThingLog { get; private set; }
        public IList<TeamUser> Teams { get; private set; }
        public int Color { get; set; }
    }
}