using System;
using System.Collections.Generic;
using Nancy;
using Nancy.Responses.Negotiation;
using Nancy.ViewEngines;

namespace HyperNotes.Api.Infrastructure {
    public class CollectionJsonProcessor : HyperNotesViewProcessor {
        public CollectionJsonProcessor(IViewFactory viewFactory) 
            : base(viewFactory, "application/vnd.collection+json", "json") {}

        public override ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context) {
            var isExactResult = requestedMediaRange.Matches(contentType)
                                || requestedMediaRange.Matches("application/json")
                                || requestedMediaRange.Matches("text/json");

            return new ProcessorMatch {
                RequestedContentTypeResult = isExactResult ? MatchResult.ExactMatch : MatchResult.NoMatch,
                ModelResult = MatchResult.DontCare
            };
        }
    } 

    public class HtmlProcessor : HyperNotesViewProcessor {
        public HtmlProcessor(IViewFactory viewFactory) 
            : base(viewFactory, "text/html", "html") {}

        public override ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context) {
            var isExactResult = requestedMediaRange.Matches(contentType)
                                || requestedMediaRange.Matches("application/html+xml");
            
            return new ProcessorMatch {
                ModelResult = MatchResult.DontCare,
                RequestedContentTypeResult = isExactResult ? MatchResult.ExactMatch : MatchResult.NoMatch
            };
        }
    }

    public abstract class HyperNotesViewProcessor : IResponseProcessor {
        
        protected readonly IViewFactory viewFactory;
        protected readonly string viewExtension;
        protected readonly string contentType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewProcessor"/> class,
        /// with the provided <paramref name="viewFactory"/>.
        /// </summary>
        /// <param name="viewFactory">The view factory that should be used to render views.</param>
        protected HyperNotesViewProcessor(IViewFactory viewFactory, string contentType, string viewExtension)
        {
            this.viewFactory = viewFactory;
            this.viewExtension = viewExtension;
            this.contentType = contentType;
        }


        /// <summary>
        /// Gets a set of mappings that map a given extension (such as .json)
        /// to a media range that can be sent to the client in a vary header.
        /// </summary>
        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings {
            get { yield return Tuple.Create(viewExtension, MediaRange.FromString(contentType)); }
        }


        /// <summary>
        /// Determines whether the the processor can handle a given content type and model.
        /// </summary>
        /// <param name="requestedMediaRange">Content type requested by the client.</param>
        /// <param name="model">The model for the given media range.</param>
        /// <param name="context">The nancy context.</param>
        /// <returns>A <see cref="ProcessorMatch"/> result that determines the priority of the processor.</returns>
        public abstract ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context);

        /// <summary>
        /// Process the response.
        /// </summary>
        /// <param name="requestedMediaRange">Content type requested by the client.</param>
        /// <param name="model">The model for the given media range.</param>
        /// <param name="context">The nancy context.</param>
        /// <returns>A <see cref="Response"/> instance.</returns>
        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context) {
            var viewName = context.NegotiationContext.ViewName + "." + viewExtension;
            var response = viewFactory.RenderView( viewName, model, GetViewLocationContext(context));
            response.ContentType = contentType;

            return response;
        }

        private static ViewLocationContext GetViewLocationContext(NancyContext context)
        {
            return new ViewLocationContext
            {
                Context = context,
                ModuleName = context.NegotiationContext.ModuleName,
                ModulePath = context.NegotiationContext.ModulePath
            };
        }
    }
}