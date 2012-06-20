using System;
using System.Linq;

namespace TeamThing.Web.Core.Security
{
    class AccessTokenValidator : IAccessTokenValidator
    {
        public bool ValidateToken(string accessToken, string[] scope)
        {
            return false;
        }
        
        public bool ValidateToken(BasicUserData userInfo)
        {
            return false;
        }
    }
}
