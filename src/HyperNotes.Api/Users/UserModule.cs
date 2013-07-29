using System;
using System.Linq;
using AutoMapper;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Raven.Client;

namespace HyperNotes.Api.Users {
    // TODO: Validation, only auth user can update, delete
    // TODO: Authentication, authenticate updates, deletes
    public class UserModule : NancyModule {
        public UserModule() {

            Get["/users"] = _ => {
                using (var db = Db.OpenSession()) {
                    var users = db.Query<UserModel>();
                    return Negotiate
                        .WithModel(users)
                        .WithView("Users/Users");
                }
            };

            Get["/users/{name}"] = param => {
                using (var db = Db.OpenSession()) {
                    var n = (string) param.name;
                    var user = db.Query<UserModel>().FirstOrDefault(u => u.UserName.Equals(n));
                    if (user != null) {
                        return Negotiate
                            .WithModel(user)
                            .WithView("Users/user");
                    }
                    
                    return HttpStatusCode.NotFound;
                }
            };
            
            Post["/users"] = data => {
                var user = Mapper.Map<NewUserModel, UserModel>(this.Bind<NewUserModel>());
                using (var db = Db.OpenSession()) {
                    db.Store(user);
                    db.SaveChanges();
                    var metadata = db.Advanced.GetMetadataFor(user);
                    var modified = metadata.Value<DateTime>("Last-Modified");
                    var etag = metadata.Value<string>("@etag");
                    
                    return new Response()
                        .WithStatusCode( HttpStatusCode.Created )
                        .WithContentType(null)
                        .WithHeaders(
                            new { header = "Location", value = "/users/" + user.UserName},
                            new { header = "Last-Modified", value = modified.ToString("r")},
                            new { header = "ETag", value = etag}
                        );
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