using System;
using System.Linq;
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
                using (var db = Infrastructure.Raven.Db.OpenSession()) {
                    var users = db.Query<NewUserModel>();
                    return View["Users.html", users];
                }
            };

            Get["/users/{name}"] = param => {
                using (var db = Db.OpenSession()) {
                    var n = (string) param.name;
                    var user = db.Query<NewUserModel>().FirstOrDefault(u => u.Name.Equals(n));
                    if (user != null) {
                        return Negotiate
                            .WithModel(user)
                            .WithView("user");
                    }
                    
                    return HttpStatusCode.NotFound;
                }
            };
            
            Post["/users"] = data => {
                var user = this.Bind<NewUserModel>();

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
                            new { header = "Location", value = "/users/" + user.Name},
                            new { header = "Last-Modified", value = modified.ToString("r")},
                            new { header = "ETag", value = etag}
                        );
                }
                
               
            };

            Put["/users/{name}"] = param => {
                using (var db = Db.OpenSession()) {
                    var putUser = this.Bind<NewUserModel>();
                    var user = db.Query<NewUserModel>().FirstOrDefault(u => u.Name.Equals(putUser.Name));
                    if (user != null) {
                        user.Name = putUser.Name;
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
                    var deleteUser = this.Bind<NewUserModel>();
                    var user = db.Query<NewUserModel>().FirstOrDefault(u => u.Name.Equals(deleteUser.Name));
                    if (user != null) {
                        db.Delete(user);
                        db.SaveChanges();
                        return 200;
                    } else {
                        return 404;
                    }
                }
            };
        }

        private static IDocumentStore Db {
            get { return Infrastructure.Raven.Db; }
        }
       
    }

}