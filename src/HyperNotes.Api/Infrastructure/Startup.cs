using Owin;

namespace HyperNotes.Api.Infrastructure {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            app.UseNancy();
        }
    }
}