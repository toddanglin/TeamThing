using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mercury.Data.Util
{
    public static class EmbeddedResourceHelper
    {
        /// <summary>
        /// Extracts an embedded file out of a given assembly.
        /// </summary>
        /// <remarks>Originally from: http://www.csharper.net/blog/getting_an_embedded_resource_file_out_of_an_assembly.aspx </remarks>
        /// <param name="assemblyName">The namespace of you assembly.</param>
        /// <param name="fileName">The name of the file to extract.</param>
        /// <returns>A stream containing the file data.</returns>
        public static Stream GetEmbeddedFile(string assemblyName, string fileName)
        {
            try
            {
                System.Reflection.Assembly a = System.Reflection.Assembly.Load(assemblyName);
                Stream str = a.GetManifestResourceStream(assemblyName + "." + fileName);

                if (str == null)
                    throw new Exception("Could not locate embedded resource '" + fileName + "' in assembly '" + assemblyName + "'");
                return str;
            }
            catch (Exception e)
            {
                throw new Exception(assemblyName + ": " + e.Message);
            }
        }

        public static Stream GetEmbeddedFile(Type type, string fileName)
        {
            string assemblyName = type.Assembly.GetName().Name;
            return GetEmbeddedFile(assemblyName, fileName);
        }
    }
}
