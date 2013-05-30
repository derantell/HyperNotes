using Owin;
    
namespace HyperNotes.Api {
    // OWIN startup
    public class Startup {
        public void Configuration(IAppBuilder app) {
            app.UseNancy();
        }
    }
}