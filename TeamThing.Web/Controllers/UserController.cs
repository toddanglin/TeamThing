using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Security;
using TeamThing.Web.Core.Helpers;
using TeamThing.Web.Core.Mappers;
using TeamThing.Web.Core.Security;
using DomainModel = TeamThing.Model;
using ServiceModel = TeamThing.Web.Models.API;
using System.Net.Http.Headers;

namespace TeamThing.Web.Controllers
{
    public class UserController : ApiController
    {
        private readonly TeamThing.Model.TeamThingContext context;

        public UserController()
        {
            this.context = new TeamThing.Model.TeamThingContext();
        }

        public IQueryable<ServiceModel.UserBasic> Get()
        {
            return context.GetAll<DomainModel.User>().MapToServiceModel();
        }

        // GET /api/user/5
        [Authorize]
        public HttpResponseMessage Get(int id)
        {
            var item = context.GetAll<TeamThing.Model.User>()
                           .FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                ModelState.AddModelError("", "Invalid User");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var sUser = item.MapToServiceModel();
            var response = new HttpResponseMessage<ServiceModel.User>(sUser, HttpStatusCode.OK);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/user/" + sUser.Id.ToString());
            return response;
        }

        // GET /api/user/5/
        [Authorize]
        public IEnumerable<ServiceModel.Thing> Things(int id, int teamId)
        {
            var item = context.GetAll<TeamThing.Model.User>()
                           .FirstOrDefault(t => t.Id == id);

            var team = context.GetAll<TeamThing.Model.Team>()
                           .FirstOrDefault(t => t.Id == teamId);

            var things = team.TeamThings.Where(t => t.AssignedTo.Any(at => at.AssignedToUserId == id));

            return things.MapToServiceModel();
        }

        [HttpPost, Obsolete("Use oauth sign in")]
        public HttpResponseMessage SignIn(ServiceModel.SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var existingUser = context.GetAll<DomainModel.User>()
                                   .FirstOrDefault(u => u.EmailAddress.Equals(model.EmailAddress, StringComparison.OrdinalIgnoreCase));

            if (existingUser == null)
            {
                ModelState.AddModelError("", "A user does not exist with this user name.");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            return new HttpResponseMessage<ServiceModel.User>(existingUser.MapToServiceModel());
        }

        [HttpPost]
        public HttpResponseMessage OAuth(ServiceModel.OAuthSignInModel model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var provider = AuthFactory.GetProvider(model.Provider, model.AuthToken);

            var userInfo = provider.GetUser();

            string userId = userInfo.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                ModelState.AddModelError("", string.Format("{0} could not locate a user using the provided auth token."));
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.Unauthorized);
            }

            var user = context.GetAll<DomainModel.User>()
                              .FirstOrDefault(u => u.OAuthProvider.Equals(model.Provider, StringComparison.OrdinalIgnoreCase) && u.OAuthUserId.Equals(userId, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                //try to find users by existing email address (mostly to clean up v1)
                if (!string.IsNullOrWhiteSpace(userInfo.Email))
                {
                    user = context.GetAll<DomainModel.User>()
                                  .FirstOrDefault(u => u.EmailAddress.Equals(userInfo.Email, StringComparison.OrdinalIgnoreCase));
                }

                //user really is new, lets create them
                if (user == null)
                {
                    user = new DomainModel.User(model.Provider, userId);
                    context.Add(user);
                }

                user.EmailAddress = userInfo.Email;
                user.ImagePath = userInfo.PictureUrl;

                if (string.IsNullOrWhiteSpace(user.ImagePath))
                {
                    var defaultImage = new Uri(Request.RequestUri, "/images/GenericUserImage.gif");
                    user.ImagePath = defaultImage.ToString();
                }

                context.SaveChanges();
            }

            FormsAuthentication.SetAuthCookie(user.EmailAddress, true);
            return new HttpResponseMessage<ServiceModel.User>(user.MapToServiceModel());
        }

        [HttpPost, Obsolete("Use oauth sign in")]
        public HttpResponseMessage Register(ServiceModel.AddUserModel value)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var existingUser = context.GetAll<DomainModel.User>()
                                   .FirstOrDefault(u => u.EmailAddress.Equals(value.EmailAddress, StringComparison.OrdinalIgnoreCase));

            if (existingUser != null)
            {
                ModelState.AddModelError("", "A user with this email address has already registered!");
                return new HttpResponseMessage<JsonValue>(ModelState.ToJson(), HttpStatusCode.BadRequest);
            }

            var user = new DomainModel.User(value.EmailAddress);
            var defaultImage = new Uri(Request.RequestUri, "/images/GenericUserImage.gif");
            user.ImagePath = defaultImage.ToString();
            context.Add(user);
            context.SaveChanges();

            var sUser = user.MapToServiceModel();
            var response = new HttpResponseMessage<ServiceModel.User>(sUser, HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/user/" + sUser.Id.ToString());
            return response;
        }

        // PUT /api/user/5
        [Authorize]
        public void Put(int id, string value)
        {
        }

        // DELETE /api/user/5
        [Authorize]
        public void Delete(int id)
        {
            //TODO: Mark inactive
        }
    }
}
