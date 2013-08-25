using Nancy;

namespace HyperNotes.Api.Root {
    public class ApiRootModule : NancyModule {
        public ApiRootModule() : base("/") {
            Get["/"] = _ => Negotiate
                                .WithStatusCode(HttpStatusCode.OK)
                                .WithView("Root/Representations/ApiRoot");
        }
    }
}