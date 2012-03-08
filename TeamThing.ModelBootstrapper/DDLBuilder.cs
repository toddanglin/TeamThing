using System;
using System.IO;
using System.Linq;
using Telerik.OpenAccess;

namespace TeamThing.ModelBootstrapper
{
    public static class DDLBuilder
    {
        public static string GenerateAndExecuteScript<T>(TextWriter log = null) where T : OpenAccessContext, new()
        {
            string ddlScript = string.Empty;
            try
            {
                using (T context = new T())
                {
                    var handler = context.GetSchemaHandler();
                    ddlScript = GenerateScript(handler, log);
                    ExecuteScript(ddlScript, handler, log);
                }
            }
            catch (Exception ex)
            {
                TryLogMessage(log, "Something went wrong..." + ex.Message);
            }
            finally
            {
                TryLogMessage(log, "Done generating and executing script.");
            }

            return ddlScript;
        }

        public static string GenerateScript<T>(TextWriter log = null) where T : OpenAccessContext, new()
        {
            string ddlScript = string.Empty;
            try
            {
                using (T context = new T())
                {
                    var handler = context.GetSchemaHandler();
                    ddlScript = GenerateScript(handler, log);
                }
            }
            catch (Exception ex)
            {
                TryLogMessage(log, "Something went wrong..." + ex.Message);
            }
            finally
            {
                TryLogMessage(log, "Done generating script.");
            }

            return ddlScript;
        }

        public static void ExecuteScript<T>(string ddlScript, TextWriter log = null) where T : OpenAccessContext, new()
        {
            try
            {
                using (T context = new T())
                {
                    var handler = context.GetSchemaHandler();
                    ExecuteScript(ddlScript, handler, log);
                }
            }
            catch (Exception ex)
            {
                TryLogMessage(log, "Something went wrong..." + ex.Message);
            }
            finally
            {
                TryLogMessage(log, "Done executing script.");
            }
        }

        private static void ExecuteScript(string ddlScript, ISchemaHandler handler, TextWriter log)
        {
            if (!string.IsNullOrEmpty(ddlScript))
            {
                if (!handler.DatabaseExists())
                {
                    TryLogMessage(log, "Database does not exist, creating database..");
                    handler.CreateDatabase();
                }

                TryLogMessage(log, "Updating database schema...");
                //This is the call that modifies the databaschema. Use with care.
                handler.ExecuteDDLScript(ddlScript);
            }
            else
            {
                TryLogMessage(log, "No changes to make.");
            }
        }



        private static string GenerateScript(ISchemaHandler handler, TextWriter log)
        {
            string ddlScript = string.Empty;

            if (!handler.DatabaseExists())
            {
                TryLogMessage(log, "Creating database script...");
                ddlScript = handler.CreateDDLScript();
            }
            else
            {
                TryLogMessage(log, "Database exists, creating migration script...");
                ddlScript = handler.CreateUpdateDDLScript(
                    new Telerik.OpenAccess.SchemaUpdateProperties());
            }
            return ddlScript;
        }

        private static void TryLogMessage(TextWriter log, string message)
        {
            if (log != null)
            {
                log.WriteLine(message);
            }
        }
    }

}
