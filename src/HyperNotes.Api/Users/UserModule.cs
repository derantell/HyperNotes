using System;
using System.Linq;
using AutoMapper;
using HyperNotes.Api.Errors;
using HyperNotes.Api.Infrastructure;
using HyperNotes.CollectionJson;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Raven.Client;

namespace HyperNotes.Api.Users {
    // TODO: Validation, only auth user can update, delete
    // TODO: Authentication, authenticate updates, deletes
    public class UserModule : NancyModule {
        public UserModule() {
            this.RequiresAuthentication();

            Get["/users"] = _ => {
                using (var db = Db.OpenSession()) {
                    var users = db.Query<UserModel>();
                    return Negotiate
                        .WithModel(users)
                        .WithView("Users/Html/Users");
                }
            };

            Get["/users/{name}"] = param => {
                using (var db = Db.OpenSession()) {
                    var n = (string) param.name;
                    var user = db.FindUser(n);
                    if (user != null) {
                        return Negotiate
                            .WithModel(user)
                            .WithView("Users/Html/User");
                        }
                    
                    return Negotiate.WithError(HttpStatusCode.NotFound, "No such user");
                }
            };
            
            Post["/users"] = _ => {
                var postedUser = this.Bind<NewUserModel>();
                var user = Mapper.Map<NewUserModel, UserModel>(postedUser);

                using (var db = Db.OpenSession()) {
                    var existingUser = db.FindUser(user.UserName);

                    if (existingUser != null) {
                        return Negotiate.WithError(HttpStatusCode.Conflict, title: "Username exists");
                    }

                    user.PwHash = UserValidationHelper.GetHash(postedUser.Password);

                    db.Store(user);
                    db.SaveChanges();

                    return new Response()
                        .WithStatusCode(HttpStatusCode.Created)
                        .WithContentType(null)
                        .WithHeader( "Location", "/users/" + user.UserName )
                        .WithHeaders( db.GetCacheHeaders(user) );
                }
            };

            Put["/users/{name}"] = param => {
                using (var db = Db.OpenSession()) {
                    var putUser = this.Bind<UserModel>();
                    var user = db.Query<UserModel>().FirstOrDefault(u => u.UserName.Equals(putUser.UserName));
                    if (user != null) {
                        user.UserName = putUser.UserName;
                        user.Email = putUser.Email;
                        db.Store(user);
                        db.SaveChanges();
                        return  View["User.html", user];
                    } else {
                        return HttpStatusCode.NotFound;
                    }
                }
            };

            Delete["/users/{name}"] = param => {
                using (var db = Db.OpenSession()) {
                    var userName = (string)param.name;
                    var user = db.Query<UserModel>().FirstOrDefault(u => u.UserName.Equals(userName));
                    if (user != null) {
                        db.Delete(user);
                        db.SaveChanges();
                        return HttpStatusCode.OK;
                    } else {
                        return HttpStatusCode.NotFound;
                    }
                }
            };
        }

        private static IDocumentStore Db {
            get { return Infrastructure.RavenDb.Store; }
        }
       
    }

}