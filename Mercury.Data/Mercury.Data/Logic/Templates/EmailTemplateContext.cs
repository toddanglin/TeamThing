using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Model;
using Mercury.Data.Util;

namespace Mercury.Data.Logic
{
    public class DefaultTemplateContext : ITemplateContext
    {
        public DefaultTemplateContext()
        {
            Things = new List<IThing>();
        }

        public string FirstName { get; set; }
        public string ToAddress { get; set; }
        public List<IThing> Things{get;set;}
        public TeamMember TeamMember { get; set; }

        public string Opening
        {
            get { return RandomPhrase.GetRandomGreeting(); }
        }

        public string Closing
        {
            get { return RandomPhrase.GetRandomClosing(); }
        }
    }

    public class ApiKeyTemplateContext : DefaultTemplateContext
    {
        public ApiKeyTemplateContext() : base() { }

        public Guid ApiKey { get; set; }
    }
}
