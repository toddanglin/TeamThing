using System;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace TeamThing.Web.Core.Security
{
    class AccessTokenValidator : IAccessTokenValidator
    {
        public bool ValidateToken(string accessToken, string[] scope)
        {
            return false;
        }
    }
}
