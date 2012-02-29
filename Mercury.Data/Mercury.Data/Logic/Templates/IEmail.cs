using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Model;

namespace Mercury.Data.Logic
{
    public interface IEmail
    {
        string Subject { get; set; }
        string Template { get; set; }
        string ResourcePath { get; set; }
        EmailType EmailType { get; set; }
        ITemplateContext Context { get; set; }

        void LoadTemplate();
    }
}
