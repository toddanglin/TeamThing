using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.SessionState;

namespace TeamThing.Web.Core.Security
{
    public class RequireOAuthAuthorization : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            string query = request.RequestUri.Query;
            IEnumerable<string> providers;
            IEnumerable<string> keys;

            request.Headers.TryGetValues("X-AuthProvider", out providers);
            string authProvider = providers.FirstOrDefault();

            request.Headers.TryGetValues("X-AuthToken", out keys);
            string authToken = keys.FirstOrDefault();

            //HACK:Unsecure mode ...haha
            string unsecureMode = HttpUtility.ParseQueryString(query).Get("noAuth");

            //HACK: we first check for the hacky unsecure mode for testing end points...
            //TODO: in the future we should probably set a flag on the request to return canned results in this case
            if (unsecureMode == null)
            {
                //now we try to find the access token in the headers
                if (authToken == null || authProvider == null)
                {
                    NotAuthorized();
                }
                else
                {
                    CheckAuthInfoCore(authToken, authProvider);
                }
            }

            base.OnActionExecuting(actionContext);
        }

        private void CheckAuthInfoCore(string authToken, string authProvider)
        {
            var userToken = UserTokenManager.Current.Get(authToken, authProvider);
            if (userToken == null)
            {
                userToken = UserTokenManager.Current.Add(authToken, authProvider);
            }

            if (userToken.IsValidationRequired())
            {
                ValidateToken(userToken);
            }
        }

        private void ValidateToken(UserTokenEntry userToken)
        {
            var provider = AuthFactory.GetProvider(userToken.Provider, userToken.Token);
            bool validToken = provider.IsTokenValid();

            if (!validToken)
            {
                NotAuthorized("This token is not valid, please refresh token or obtain valid token!");
            }
            else
            {
                userToken.LastValidated = DateTime.Now;
            }
        }

        private void NotAuthorized(string message = "You must supply valid token to access method!")
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(message),
                StatusCode = HttpStatusCode.Unauthorized
            };
            throw new HttpResponseException(response);
        }
    }

    public static class UserTokenManager
    {
        private static IUserTokenManager manager = new SessionBasedUserTokenManager();

        public static IUserTokenManager Current
        {
            get
            {
                return manager;
            }
        }
    }

    public class UserTokenEntry
    {
        //TODO: Read from config
        private readonly TimeSpan validationExpiry = new TimeSpan(3, 0, 0);

        public string Token
        {
            get;
            set;
        }

        public DateTime? LastValidated
        {
            get;
            set;
        }

        public string Provider
        {
            get;
            set;
        }

        public bool IsValidationRequired()
        {
            if (LastValidated == null || LastValidated.Value.Add(validationExpiry) <= DateTime.Now)
            {
                return true;
            }

            return false;
        }

    }

    public interface IUserTokenManager
    {
        bool Contains(string token, string provider);
        UserTokenEntry Get(string token, string provider);
        void Add(UserTokenEntry entry);
        UserTokenEntry Add(string token, string provider);
    }

    public class SessionBasedUserTokenManager : IUserTokenManager
    {
        private readonly HttpSessionState session;

        public SessionBasedUserTokenManager()
            : this(HttpContext.Current.Session)
        {
        }

        public SessionBasedUserTokenManager(HttpSessionState session)
        {
            this.session = session;
        }

        public UserTokenEntry Add(string token, string provider)
        {
            var sessionKey = BuildTokenKey(token, provider);

            var userToken = new UserTokenEntry
            {
                Token = token,
                Provider = provider
            };

            session[sessionKey] = userToken;

            return userToken;
        }

        public bool Contains(string token, string provider)
        {
            var userToken = GetUserTokenFromSession(token, provider);
            if (userToken != null)
            {
                return true;
            }

            return false;
        }

        private UserTokenEntry GetUserTokenFromSession(string token, string provider)
        {
            var sessionKey = BuildTokenKey(token, provider);
            var userToken = session[sessionKey] as UserTokenEntry;
            return userToken;
        }

        public UserTokenEntry Get(string token, string provider)
        {
            return GetUserTokenFromSession(token, provider);
        }

        public void Add(UserTokenEntry entry)
        {
            var sessionKey = BuildTokenKey(entry.Token, entry.Provider);
            session[sessionKey] = entry;
        }

        public string BuildTokenKey(string token, string provider)
        {
            return string.Format("{0}-{1}-{2}", "UserToken", provider, token);
        }
    }
}