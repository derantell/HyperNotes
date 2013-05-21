using System.Linq;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.ModelBinding;
using Nancy.TinyIoc;
using Owin;
using Raven.Client;
using Raven.Client.Document;

namespace HyperNotes.Api {
    // TODO: Validation, only auth user can update, delete
    // TODO: Authentication, authenticate updates, deletes
    public class UserModule : NancyModule {
        public UserModule() {
            Get["/users"] = _ => {
                using (var db = Raven.Db.OpenSession()) {
                    var users = db.Query<NewUserModel>();
                    return View["Users.html", users];
                }
            };

            Get["/users/{name}"] = param => {
                using (var db = Raven.Db.OpenSession()) {
                    var n = (string) param.name;
                    var user = db.Query<NewUserModel>().FirstOrDefault(u => u.Name.Equals(n));
                    if (user != null)
                        return View["User.html", user];
                    else
                        return 404;
                }
            };
            
            Post["/users"] = data => {
                var user = this.Bind<NewUserModel>();

                using (var db = Raven.Db.OpenSession()) {
                    db.Store(user);
                    db.SaveChanges();
                }

                Context.Response.Headers.Add("location", "/users/" + user.Name);
                return 201;
            };

            Put["/users/{name}"] = param => {
                using (var db = Raven.Db.OpenSession()) {
                    var putUser = this.Bind<NewUserModel>();
                    var user = db.Query<NewUserModel>().FirstOrDefault(u => u.Name.Equals(putUser.Name));
                    if (user != null) {
                        user.Name = putUser.Name;
                        user.Email = putUser.Email;
                        db.Store(user);
                        db.SaveChanges();
                        return View["User.html", user];
                    } else {
                        return 404;
                    }
                }
            };

            Delete["/users/{name}"] = param => {
                using (var db = Raven.Db.OpenSession()) {
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
    }

    public class UserModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PwHash { get; set; }
    }

    public class NewUserModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Bootstrapping Nancy
    public class HyperNoteBootstrapper : DefaultNancyBootstrapper {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            Raven.Init("http://derantell-pc:8080");
        }
    }

    // For katana OWIN 
    public class Startup {
        public void Configuration(IAppBuilder app) {
            app.UseNancy();
        }
    }

    // The ravenDB document store
    public static class Raven {
        public static IDocumentStore Db { get; private set; }

        public static void Init(string url) {
            Db = new DocumentStore {Url = url};
            Db.Initialize();
        }
    }
}