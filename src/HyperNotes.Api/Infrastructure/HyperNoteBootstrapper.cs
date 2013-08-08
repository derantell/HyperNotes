using AutoMapper;
using HyperNotes.Api.Users;
using Nancy;
using Nancy.Authentication.Basic;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace HyperNotes.Api.Infrastructure {
    public class HyperNoteBootstrapper : DefaultNancyBootstrapper {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            pipelines.EnableBasicAuthentication(
                new BasicAuthenticationConfiguration(
                    userValidator: new UserValidator(), 
                    realm: "HyperNotes"
                )
            );
            
            RavenDb.Init("http://derantell-pc:8080");
            
            Mapper.CreateMap<NewUserModel, UserModel>();
        }
    }
}