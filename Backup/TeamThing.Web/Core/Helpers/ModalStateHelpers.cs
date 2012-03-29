using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Json;
using System.Web.Http.ModelBinding;

namespace TeamThing.Web.Core.Helpers
{
    public static class ModalStateHelpers
    {
        public static JsonArray ToJson(this ModelStateDictionary modalState)
        {
            var errors = new JsonArray();
            foreach (var prop in modalState.Values)
            {
                if (prop.Errors.Any())
                {
                    errors.Add(prop.Errors.First().ErrorMessage);
                }
            }

            return errors;
        }
    }
}