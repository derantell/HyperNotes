using System.Text.RegularExpressions;
using AutoMapper;
using HyperNotes.Api.Notes;
using HyperNotes.Api.Persistance;
using HyperNotes.Api.Users;
using Nancy;
using Nancy.Authentication.Basic;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
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
            
            Mapper.CreateMap<UserDto, User>();
            Mapper.CreateMap<NoteDto, Note>()
                  .ForMember(
                      model => model.Tags,
                      opt => opt.MapFrom(dto => Regex.Split(dto.Tags, @"[ ;,]+")));

        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get {
                var responseProcessors = new[] {
                    typeof (HtmlProcessor),
                    typeof (CollectionJsonProcessor),
                    typeof (AtomProcessor),
                    typeof (MarkdownProcessor)
                };

                return NancyInternalConfiguration
                    .WithOverrides(b => b.ResponseProcessors = responseProcessors);
            }
        }

        protected override Nancy.Diagnostics.DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get
            {
               return new DiagnosticsConfiguration{Password = "foobar"};
            }
        }
    }
}