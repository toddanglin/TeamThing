﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TeamThing.Web.Core.Mappers;
using DomainModel = TeamThing.Model;
using ServiceModel = TeamThing.Web.Models.API;

namespace TeamThing.Web.Controllers
{
    public class UserController : ApiController
    {
        // GET /api/user
        private TeamThing.Model.TeamThingContext context;

        public UserController()
        {
            this.context = new TeamThing.Model.TeamThingContext();
        }

        public IQueryable<ServiceModel.UserBasic> Get()
        {
            return context.GetAll<DomainModel.User>().MapToServiceModel();
        }

        // GET /api/user/5
        public ServiceModel.User Get(int id)
        {
            return context.GetAll<TeamThing.Model.User>()
                          .First(t => t.Id == id)
                          .MapToServiceModel();
        }

        [HttpPost]
        public HttpResponseMessage SignIn(ServiceModel.SignInViewModel model)
        {
            // Validate movie
            if (!ModelState.IsValid)
            {
                var errors = new JsonArray();
                foreach (var prop in ModelState.Values)
                {
                    if (prop.Errors.Any())
                    {
                        errors.Add(prop.Errors.First().ErrorMessage);
                    }
                }
                return new HttpResponseMessage<JsonValue>(errors, HttpStatusCode.BadRequest);
            }

            var existingUser = context.GetAll<DomainModel.User>()
                             .FirstOrDefault(u => u.EmailAddress == model.EmailAddress);

            if (existingUser == null)
            {
                var errors = new JsonArray();
                errors.Add("A user does not exist with this user name.");
                return new HttpResponseMessage<JsonValue>(errors, HttpStatusCode.BadRequest);
            }

            return new HttpResponseMessage<ServiceModel.User>(existingUser.MapToServiceModel());
        }

        public HttpResponseMessage Register(ServiceModel.AddUserModel value)
        {
            // Validate movie
            if (!ModelState.IsValid)
            {
                var errors = new JsonArray();
                foreach (var prop in ModelState.Values)
                {
                    if (prop.Errors.Any())
                    {
                        errors.Add(prop.Errors.First().ErrorMessage);
                    }
                }
                return new HttpResponseMessage<JsonValue>(errors, HttpStatusCode.BadRequest);
            }

            var existingUser = context.GetAll<DomainModel.User>()
                              .FirstOrDefault(u => u.EmailAddress == value.EmailAddress);

            if (existingUser != null)
            {
                var errors = new JsonArray();
                errors.Add("A user with this email address has already registered!");
                return new HttpResponseMessage<JsonValue>(errors, HttpStatusCode.BadRequest);
            }

            var user = new DomainModel.User(value.EmailAddress);
            context.Add(user);
            context.SaveChanges();

            var sUser = user.MapToServiceModel();
            var response = new HttpResponseMessage<ServiceModel.User>(sUser, HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/user/" + sUser.Id.ToString());
            return response;
        }

        // PUT /api/user/5
        public void Put(int id, string value)
        {
        }

        // DELETE /api/user/5
        public void Delete(int id)
        {
        }

       
    }
}