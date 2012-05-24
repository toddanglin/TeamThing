using System;

namespace TeamThing.Web.Controllers
{
    public class AuthFactory
    {
        public static IOAuthProvider GetProvider(string provider, string authToken)
        {
            switch (provider.ToLowerInvariant())
            {
                case "google":
                    return new GoogleAuthProvider(authToken);
                    break;
                case "facebook":
                    return new FacebookAuthProvider(authToken);
                    break;
            }

            throw new Exception("Unsupported OAuth provider " + provider);
        }
    }
}