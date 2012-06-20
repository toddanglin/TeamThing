using System;
using System.Json;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace TeamThing.Web.Core.Security
{
    public class FacebookAuthProvider : IOAuthProvider
    {
        private static readonly Uri userInfoBaseUrl=new Uri("https://graph.facebook.com/me");
        private readonly string accessToken;
        public FacebookAuthProvider(string accessToken)
        {
            this.accessToken=accessToken;
        }

        public BasicUserData GetUser()
        {
            var builder=new UriBuilder(userInfoBaseUrl) { Query=string.Format("fields={0}&access_token={1}", "id,name,email,picture", accessToken) };

            var profileClient=new HttpClient();
            HttpResponseMessage profileResponse=profileClient.GetAsync(builder.Uri).Result;

            var formatter=new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
            var userInfo=profileResponse.Content.ReadAsAsync<JToken>(new[] { formatter }).Result;
            return
            new BasicUserData
            {
                UserId=userInfo["id"].Value<string>(),
                UserName=userInfo["name"].Value<string>(),
                PictureUrl=userInfo["picture"].Value<string>(),
                Email=userInfo["email"].Value<string>()
            };
        }

        public bool IsTokenValid()
        {
            //easiest way for face book is to simply try and access the graph
            var builder=new UriBuilder(userInfoBaseUrl) { Query=string.Format("fields={0}&access_token={1}", "id,name,email,picture", accessToken) };

            var profileClient=new HttpClient();
            HttpResponseMessage profileResponse=profileClient.GetAsync(builder.Uri).Result;

            var formatter=new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
            var userInfo = profileResponse.Content.ReadAsAsync<JToken>(new[] { formatter }).Result;

            if (userInfo["error"] != null)
            {
                return false;
            }

            return true;
        }
    }
}