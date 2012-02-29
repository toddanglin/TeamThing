using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Model;
using Telerik.OpenAccess;
using ImapX;

namespace Mercury.Data.Logic
{
    public interface IThingProcessor
    {
        IEmail Process(string thingData, TeamMember teamMember, Message message, IObjectScope scope);
    }
}
