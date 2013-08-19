using AutoMapper;
using HyperNotes.Api.Infrastructure;
using HyperNotes.Api.Persistance;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace HyperNotes.Api.Users {
    public class OpenUserModule : NancyModule {
        public OpenUserModule() : base("/users") {

            Get["/"] = _ => {
                using (var db = RavenDb.Store.OpenSession()) {
                    var users = new FunctionalList<User>( db.Query<User>() );
                    return Negotiate
                        .WithModel(users)
                        .WithView("Users/Representations/List");
                }
            };

            Get["/{name}"] = param => {
                using (var db = RavenDb.Store.OpenSession()) {
                    var n = (string) param.name;
                    var user = db.FindUser(n);
                    if (user != null) {
                        return Negotiate
                            .WithModel(user)
                            .WithHeaders(db.GetCacheHeaders(user))
                            .WithView("Users/Representations/Single");
                        }
                    
                    return Negotiate.WithError(HttpStatusCode.NotFound, "No such user");
                }
            };
            
            Post["/"] = _ => {
                var postedUser = this.Bind<UserDto>();
                var user = Mapper.Map<UserDto, User>(postedUser);

                using (var db = RavenDb.Store.OpenSession()) {
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

        }
   }
    
    public class SecureUserModule : NancyModule {
        public SecureUserModule() : base("/users") {
            this.RequiresAuthentication();
            
            Put["/{name}"] = param => {
                var resourceName = (string)param.name;
                var requestData = this.Bind<UserDto>();
                var boundUser = Mapper.Map<UserDto, User>(requestData);

                using (var db = RavenDb.Store.OpenSession()) {
                    var user = db.FindUser(resourceName);
                    if (user == null) {
                        return Negotiate.WithError(HttpStatusCode.NotFound, "No such user");
                    }

                    if (!UserValidationHelper.IsLoggedInUser(Context.CurrentUser.UserName, resourceName)) {
                        return Negotiate.WithError(HttpStatusCode.Forbidden, "Forbidden");
                    }

                    if (resourceName != requestData.UserName) {
                        return Negotiate.WithError(HttpStatusCode.Forbidden, "Cannot change user name");
                    }

                    user.Email = boundUser.Email;

                    db.Store(user);
                    db.SaveChanges();

                    return Negotiate
                        .WithModel(user)
                        .WithHeaders( db.GetCacheHeaders(user) )
                        .WithView("Users/Representations/Single");
                }
            };

            Delete["/{name}"] = param => {
                var resourceName = (string)param.name;

                using (var db = RavenDb.Store.OpenSession()) {
                    if (!UserValidationHelper.IsLoggedInUser(Context.CurrentUser.UserName, resourceName)) {
                        return Negotiate.WithError(HttpStatusCode.Forbidden, "Forbidden");
                    }

                    var user = db.FindUser(resourceName);

                    if (user != null) {
                        db.Delete(user);
                        db.SaveChanges();
                    }

                    return HttpStatusCode.OK;
                }
            };
        }
        
    }

}