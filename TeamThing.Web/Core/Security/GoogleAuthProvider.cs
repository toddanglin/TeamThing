using System;
using System.Json;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json.Linq;

namespace TeamThing.Web.Controllers
{
    public class GoogleAuthProvider : IOAuthProvider
    {
        private static readonly Uri userInfoBaseUrl = new Uri("https://www.googleapis.com/oauth2/v1/userinfo");
        private static readonly Uri tokenValidatorBaseUrl = new Uri("https://www.googleapis.com/oauth2/v1/tokeninfo");
        private readonly string accessToken;
        //TODO: store in config
        private string clientAppId = "1071592151045.apps.googleusercontent.com";
        public GoogleAuthProvider(string accessToken)
        {
            this.accessToken = accessToken;
        }

        public bool IsTokenValid()
        {
            var tokenValidatorUrl = new UriBuilder(tokenValidatorBaseUrl)
            {
                Query = string.Format("access_token={1}", accessToken)
            };

            var client = new HttpClient();
            HttpResponseMessage validationResponse = client.GetAsync(tokenValidatorUrl.Uri).Result;

            var formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
            var responseData = validationResponse.Content.ReadAsAsync<JsonValue>(new[] { formatter }).Result;

            var error = responseData["error"].ReadAs<string>(null);
            var audience = responseData["audience"].ReadAs<string>(null);

            //if an error was returned, the token is invalid
            if (error != null)
            {
                return false;
            }
            //if the audience does not match our client id, then something is off (confused deputy) force re-auth
            else if (audience == null || audience != clientAppId)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public BasicUserData GetUser()
        {
            var builder = new UriBuilder(userInfoBaseUrl)
            {
                Query = string.Format("access_token={0}", Uri.EscapeDataString(accessToken))
            };

            var profileClient = new HttpClient();
            HttpResponseMessage profileResponse = profileClient.GetAsync(builder.Uri).Result;

            var formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
            var userInfo = profileResponse.Content.ReadAsAsync<JToken>(new[] { formatter }).Result;

            return
            new BasicUserData
            {
                UserId = userInfo["id"].Value<string>(),
                UserName = userInfo["name"].Value<string>(),
                PictureUrl = userInfo["picture"].Value<string>(),
                Email = userInfo["email"].Value<string>()
            };
        }
    }
 }