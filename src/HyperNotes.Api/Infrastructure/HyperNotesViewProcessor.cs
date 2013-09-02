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
            var isExactResult = requestedMediaRange.Matches(ContentType)
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
            var isExactResult = requestedMediaRange.Matches(ContentType)
                                || requestedMediaRange.Matches("application/xhtml+xml");
            
            return new ProcessorMatch {
                ModelResult = MatchResult.DontCare,
                RequestedContentTypeResult = isExactResult ? MatchResult.ExactMatch : MatchResult.NoMatch
            };
        }
    }

    public class MarkdownProcessor : HyperNotesViewProcessor {
        public MarkdownProcessor(IViewFactory viewFactory) 
            : base(viewFactory, "text/x-markdown", "md") {}

        public override ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context) {
            var isExactResult = requestedMediaRange.Matches(ContentType) 
                || requestedMediaRange.Matches("text/markdown")
                || requestedMediaRange.Matches("text/plain");

            return new ProcessorMatch {
                ModelResult = MatchResult.DontCare,
                RequestedContentTypeResult = isExactResult ? MatchResult.ExactMatch : MatchResult.NoMatch
            };
        }
    }

    public class AtomProcessor : HyperNotesViewProcessor {
        public AtomProcessor(IViewFactory viewFactory) :
            base(viewFactory, "application/atom+xml", "atom") {
            
        }

        public override ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context) {
            var isExactResult = requestedMediaRange.Matches(ContentType) 
                || requestedMediaRange.Matches("application/xml")
                || requestedMediaRange.Matches("text/xml");

            return new ProcessorMatch {
                ModelResult = MatchResult.DontCare,
                RequestedContentTypeResult = isExactResult ? MatchResult.ExactMatch : MatchResult.NoMatch
            };
        }
    }



    public abstract class HyperNotesViewProcessor : IResponseProcessor {
        
        protected readonly IViewFactory ViewFactory;
        protected readonly string ViewExtension;
        protected readonly string ContentType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewProcessor"/> class,
        /// with the provided <paramref name="viewFactory"/>.
        /// </summary>
        /// <param name="viewFactory">The view factory that should be used to render views.</param>
        protected HyperNotesViewProcessor(IViewFactory viewFactory, string contentType, string viewExtension)
        {
            this.ViewFactory = viewFactory;
            this.ViewExtension = viewExtension;
            this.ContentType = contentType;
        }


        /// <summary>
        /// Gets a set of mappings that map a given extension (such as .json)
        /// to a media range that can be sent to the client in a vary header.
        /// </summary>
        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings {
            get { yield return Tuple.Create(ViewExtension, MediaRange.FromString(ContentType)); }
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
            var viewName = context.NegotiationContext.ViewName + "." + ViewExtension;
            var response = ViewFactory.RenderView( viewName, model, GetViewLocationContext(context));
            response.ContentType = ContentType;

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