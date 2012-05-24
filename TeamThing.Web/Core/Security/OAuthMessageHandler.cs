using System;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Security;
using System.Net.Http.Formatting;

public class OAuthFacebookMessageHandler : DelegatingHandler
{
    private static readonly Uri FacebookAccessTokenBaseUri = new Uri("https://graph.facebook.com/oauth/access_token");
    private static readonly Uri FacebookBaseGraphUri = new Uri("https://graph.facebook.com/me?fields=id,name,email");

    private readonly string facebookAppId;

    private readonly string facebookAppSecret;

    public OAuthFacebookMessageHandler(string appId, string secret)
    {
        this.facebookAppId = appId;
        this.facebookAppSecret = secret;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Method == HttpMethod.Get && request.RequestUri.Segments.Last() == "authtoken")
        {
            string querystring = request.RequestUri.Query.Substring(1);
            string[] queryParams = querystring.Split(new[] { '&' });

            var queryStringParams = request.RequestUri.ParseQueryString();
            if (queryStringParams.Count > 0)
            {
                string code = queryParams.Where(p => p.StartsWith("code")).First().Split(new[] { '=' })[1];

                return Task.Factory.StartNew(
                    () =>
                    {
                        string accessToken = this.GetFacebookAccessToken(code, request);
                        string username = GetFacebookUsername(accessToken);

                        var ticket = new FormsAuthenticationTicket(username, false, 60);
                        string s = FormsAuthentication.Encrypt(ticket);

                        var response = new HttpResponseMessage();
                        response.Headers.Add("Set-Cookie", string.Format("ticket={0}; path=/", s));

                        var responseContentBuilder = new StringBuilder();
                        responseContentBuilder.AppendLine("<html>");
                        responseContentBuilder.AppendLine("   <head>");
                        responseContentBuilder.AppendLine("      <title>Login Callback</title>");
                        responseContentBuilder.AppendLine("   </head>");
                        responseContentBuilder.AppendLine("   <body>");
                        responseContentBuilder.AppendLine("      <script type=\"text/javascript\">");
                        responseContentBuilder.AppendLine(
                            "         if(window.opener){");

                        if (queryStringParams["callback"] != null)
                        {
                            responseContentBuilder.AppendLine(queryStringParams["callback"] + "();");
                        }

                        responseContentBuilder.AppendLine("            window.close()';");
                        responseContentBuilder.AppendLine("         }");
                        responseContentBuilder.AppendLine("      </script>");
                        responseContentBuilder.AppendLine("   </body>");
                        responseContentBuilder.AppendLine("</html>");

                        response.Content = new StringContent(responseContentBuilder.ToString());
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                        response.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };

                        return response;
                    });
            }

            return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.InternalServerError));
        }

        return base.SendAsync(request, cancellationToken);
    }

    private static string GetFacebookUsername(string accessToken)
    {
        var builder = new UriBuilder(FacebookBaseGraphUri) { Query = string.Format("fields={0}&access_token={1}", "id,name,email",accessToken) };

        var profileClient = new HttpClient();
        HttpResponseMessage profileResponse = profileClient.GetAsync(builder.Uri).Result;

        var formatter = new JsonMediaTypeFormatter();
        formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
        var v = profileResponse.Content.ReadAsAsync<JsonValue>(new[] { formatter }).Result;
        return v["name"].ReadAs<string>();
    }

    private string GetFacebookAccessToken(string code, HttpRequestMessage request)
    {
        var redirectUriBuilder = new UriBuilder()
        {
            Host = request.RequestUri.Host,
            Port = request.RequestUri.Port,
            Path = request.RequestUri.AbsolutePath
        };

        var uriBuilder = new UriBuilder(FacebookAccessTokenBaseUri)
        {
            Query =
                string.Format(
                    "client_id={0}&redirect_uri={1}&client_secret={2}&code={3}",
                    this.facebookAppId,
                    redirectUriBuilder.Uri,
                    this.facebookAppSecret,
                    code)
        };

        var accessTokenClient = new HttpClient();
        HttpResponseMessage accessTokenResponse = accessTokenClient.GetAsync(uriBuilder.Uri).Result;
        string responseBody = accessTokenResponse.Content.ReadAsStringAsync().Result;
        if (responseBody != null)
        {
            string accessToken = responseBody.Split(new[] { '&' }).First().Split(new[] { '=' })[1];
            return accessToken;
        }

        throw new Exception("access token response body should not be null");
    }
}