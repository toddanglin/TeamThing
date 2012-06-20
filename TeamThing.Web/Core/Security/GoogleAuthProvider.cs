using System;
using System.Json;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json.Linq;
using TeamThing.Web.Core.Security;

namespace TeamThing.Web.Core.Security
{
    public class GoogleAuthProvider : IOAuthProvider
    {
        private static readonly Uri userInfoBaseUrl = new Uri("https://www.googleapis.com/oauth2/v1/userinfo");
        private static readonly Uri tokenValidatorBaseUrl = new Uri("https://www.googleapis.com/oauth2/v1/tokeninfo");
        private readonly string accessToken;
        //TODO: store in config
        private readonly string clientAppId="1071592151045.apps.googleusercontent.com";

        public GoogleAuthProvider(string accessToken)
        {
            this.accessToken = accessToken;
        }

        public bool IsTokenValid()
        {
            var tokenValidatorUrl = new UriBuilder(tokenValidatorBaseUrl)
            {
                Query = string.Format("access_token={0}", accessToken)
            };

            var client = new HttpClient();
            HttpResponseMessage validationResponse = client.GetAsync(tokenValidatorUrl.Uri).Result;

            var formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
            var responseData = validationResponse.Content.ReadAsAsync<JToken>(new[] { formatter }).Result;

            var error = responseData.Value<string>("error");
            var audience = responseData.Value<string>("audience");


            //if an error was returned, the token is invalid
            if (error != null)
            {
                return false;
            }
            //ensure the token is valid for THIS app
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
                UserId = userInfo.Value<string>("id"),
                UserName = userInfo.Value<string>("name"),
                PictureUrl = userInfo.Value<string>("picture"),
                Email = userInfo.Value<string>("email")
            };
        }
    }
}