using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImapX;
using Mercury.Model;

namespace Mercury.Data.Logic
{
    public abstract class ThingProcessorBase : IThingProcessor
    {
        public ThingProcessorBase()
        {

        }

        protected virtual IEmail CreateEmail<T>(TeamMember teamMember, IThing o)
            where T : IEmail, new()
        {
            IEmail email = null;
            email = new T();

            var context = new DefaultTemplateContext()
            {
                FirstName = teamMember.FirstName,
                TeamMember = teamMember,
                ToAddress = teamMember.Email,
            };
            context.Things.Add(o);
            email.Context = context;

            return email;
        }

        #region IThingProcessor Members

        public virtual IEmail Process(string thingData, Model.TeamMember teamMember, Message message, Telerik.OpenAccess.IObjectScope scope)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
