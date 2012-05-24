using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace TeamThing.Web.Core.Security
{
    public class RequireOAuthAuthorization : ActionFilterAttribute
    {
        public string Scope { get; set; }

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string[] scope = null;
            if (!string.IsNullOrEmpty(Scope))
            {
                scope = Scope.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            string query = actionContext.Request.RequestUri.Query;
            string accessToken = HttpUtility.ParseQueryString(query).Get("accessToken");

            // we first check for valid token
            if (accessToken != null)
            {
                IAccessTokenValidator accessTokenValidator = new AccessTokenValidator();
                bool validToken = accessTokenValidator.ValidateToken(accessToken, scope);

                if (!validToken)
                {
                    var response = new HttpResponseMessage
                    {
                        Content =
                            new StringContent("This token is not valid, please refresh token or obtain valid token!"),
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                    throw new HttpResponseException(response);
                }
            }
            else
            {
                var response = new HttpResponseMessage
                {
                    Content =
                        new StringContent("You must supply valid token to access method!"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                throw new HttpResponseException(response);
            }

            base.OnActionExecuting(actionContext);
        }
    }
  
    public interface IAccessTokenValidator
    {
        bool ValidateToken(string accessToken, string[] scope);
    }
}