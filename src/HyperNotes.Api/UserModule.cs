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
            Get["/"] = _ => "Hello user!";

            Put["/users"] = data => {
                var user = this.Bind<UserModel>();

                using (var db = Raven.Db.OpenSession()) {
                    db.Store(user);
                    db.SaveChanges();
                }

                return 201;
            };
        }
    }

    public class UserModel {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class HyperNoteBootstrapper : DefaultNancyBootstrapper {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            Raven.Init("http://derantell-pc:8080");
        }
    }

    public class Startup {
        public void Configuration(IAppBuilder app) {
            app.UseNancy();
        }
    }

    public static class Raven {
        public static IDocumentStore Db { get; private set; }

        public static void Init(string url) {
            Db = new DocumentStore {Url = url};
            Db.Initialize();
        }
    }
}