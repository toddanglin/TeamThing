using System;
using System.Collections.Generic;

namespace TeamThing.Web.Models.API
{
    public class Thing
    {
        public int Id { get;set; }
        public string Description { get;set; }
        public string Status { get;set; }
        public DateTime DateCreated { get; set; }
        public UserBasic Owner { get; set; }
        public Team Team { get; set; }
        public IEnumerable<UserBasic> AssignedTo { get; set; }
    }

    public class ThingBasic
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}