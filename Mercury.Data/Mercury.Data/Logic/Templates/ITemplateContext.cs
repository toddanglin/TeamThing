using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Model;

namespace Mercury.Data.Logic
{
    public interface ITemplateContext
    {
        TeamMember TeamMember { get; set; }
        string FirstName { get; set; }
        string ToAddress { get; set; }
        string Opening { get; }
        string Closing { get; }
        List<IThing> Things { get; set; }
    }
}
