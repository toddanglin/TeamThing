using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;

namespace TeamThing.Model
{
    public class TeamThingContext : OpenAccessContext
    {
        static MetadataContainer metadataContainer = new TeamThingMetadataSource().GetModel();
        static BackendConfiguration backendConfiguration = new BackendConfiguration()
        {
            Backend = "mssql"
        };

        private const string DbConnection = "TeamThingV2";

        public TeamThingContext()
            : base(DbConnection, backendConfiguration, metadataContainer)
        {

        }

        public IQueryable<Team> Products
        {
            get
            {
                return this.GetAll<Team>();
            }
        }

        public void UpdateSchema()
        {
            var handler = this.GetSchemaHandler();
            string script = null;
            try
            {
                script = handler.CreateUpdateDDLScript(null);
            }
            catch
            {
                bool throwException = false;
                try
                {
                    handler.CreateDatabase();
                    script = handler.CreateDDLScript();
                }
                catch
                {
                    throwException = true;
                }
                if (throwException)
                    throw;
            }

            if (string.IsNullOrEmpty(script) == false)
            {
                handler.ExecuteDDLScript(script);
            }
        }
    }
}
