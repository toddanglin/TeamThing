using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;
using TeamThing.Web;


public class OAuthFacebookOpHandler : System.Web.Http.AuthorizeAttribute
{

    private readonly string facebookAppId;

    private readonly Uri facebookBaseAuthUri = new Uri("https://www.facebook.com/dialog/oauth");

    public OAuthFacebookOpHandler()
    {
    }

    protected bool AuthorizeCore(IPrincipal principal)
    {
        if (!principal.Identity.IsAuthenticated)
        {
            return false;
        }

        string[] usersSplit = SplitString(this.Users);
        if (usersSplit.Length > 0 && !usersSplit.Contains(principal.Identity.Name, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        string[] rolesSplit = SplitString(this.Roles);
        return rolesSplit.Length <= 0 || rolesSplit.Any(principal.IsInRole);
    }

    public override void OnAuthorization(HttpActionContext actionContext)
    {
        IIdentity identity = GetIdentity();

        if (identity == null)
        {
            var challengeMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            challengeMessage.Headers.WwwAuthenticate.Add(
                new AuthenticationHeaderValue("oauth", "location=\"" + this.BuildFacebookAuthUri(actionContext.Request) + "\""));
            throw new HttpResponseException(challengeMessage);
        }

        var principle = new GenericPrincipal(identity, new string[0]);

        // set the thread context
        Thread.CurrentPrincipal = principle;

        if (!this.AuthorizeCore(principle))
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }
    }


    private static IIdentity GetIdentity()
    {
        HttpCookie ticketCookie = HttpContext.Current.Request.Cookies["ticket"];
        if (ticketCookie == null)
        {
            return null;
        }

        string val = ticketCookie.Value;
        FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(val);
        var ident = new FormsIdentity(ticket);
        return ident;
    }

    // ripped from MVC AuthorizeAttribute (http://aspnet.codeplex.com/SourceControl/changeset/view/70574#266447) 
    private static string[] SplitString(string original)
    {
        if (string.IsNullOrEmpty(original))
        {
            return new string[0];
        }

        IEnumerable<string> split = from piece in original.Split(',')
                                    let trimmed = piece.Trim()
                                    where !string.IsNullOrEmpty(trimmed)
                                    select trimmed;
        return split.ToArray();
    }

    private Uri BuildFacebookAuthUri(HttpRequestMessage request)
    {
        var queryString = request.RequestUri.ParseQueryString();
        var returnUriBuilder = new UriBuilder
        {
            Host = request.RequestUri.Host,
            Port = request.RequestUri.Port,
            Path = request.RequestUri.LocalPath.Replace(request.GetRouteData().Values["action"].ToString(), "authtoken")
        };

        if (queryString["callback"] != null)
        {
            returnUriBuilder.Query = string.Format("callback={0}", queryString["callback"]);
        }

        var builder = new UriBuilder(this.facebookBaseAuthUri)
        {
            Query =
                string.Format(
                    "client_id={0}&redirect_uri={1}&response_type=code",
                    WebApiApplication.FACEBOOK_APP_ID,
                    returnUriBuilder.Uri)
        };
        return builder.Uri;
    }


}

