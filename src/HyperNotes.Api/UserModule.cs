using Nancy;
using Nancy.Bootstrapper;
using Nancy.ModelBinding;
using Nancy.TinyIoc;
using Owin;
using Raven.Client;
using Raven.Client.Document;

namespace HyperNotes.Api {
    public class UserModule : NancyModule {
        public UserModule() {
            Get["/users"] = _ => {
                using (var db = Raven.Db.OpenSession()) {
                    var users = db.Query<NewUserModel>();
                    return View["Users.html", users];
                }
            };
            
            Put["/users/{name}"] = data => {
                var user = this.Bind<NewUserModel>();

                using (var db = Raven.Db.OpenSession()) {
                    db.Store(user);
                    db.SaveChanges();
                }

                return 201;
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