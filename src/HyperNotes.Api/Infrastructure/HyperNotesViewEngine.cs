using System.Collections.Generic;
using System.IO;
using Nancy;
using Nancy.Responses;
using Nancy.ViewEngines;
using Nancy.ViewEngines.SuperSimpleViewEngine;

namespace HyperNotes.Api.Infrastructure {
    public class HyperNotesViewEngine : IViewEngine {   
        /// <summary>
        /// Extensions that the view engine supports
        /// </summary>
        private readonly string[] extensions = new[] { "html", "json", "atom", "md"  };

        /// <summary>
        /// The engine itself
        /// </summary>
        private readonly SuperSimpleViewEngine viewEngine;

        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get { return this.extensions; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperSimpleViewEngineWrapper"/> class, using
        /// the provided <see cref="ISuperSimpleViewEngineMatcher"/> extensions.
        /// </summary>
        /// <param name="matchers">The matchers to use with the engine.</param>
        public HyperNotesViewEngine(IEnumerable<ISuperSimpleViewEngineMatcher> matchers)
        {
            this.viewEngine = new SuperSimpleViewEngine(matchers);
        }

        /// <summary>
        /// Initialise the view engine (if necessary)
        /// </summary>
        /// <param name="viewEngineStartupContext">Startup context</param>
        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <param name="renderContext">An <see cref="IRenderContext"/> instance.</param>
        /// <returns>A response</returns>
        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext) {
            renderContext.Context.Request.Url.BasePath = renderContext.Context.Request.Url.SiteBase;
            return new Response {
                Contents = s =>
                {
                    var writer = new StreamWriter(s);
                    var templateContents = renderContext.ViewCache.GetOrAdd(
                        viewLocationResult, vr => vr.Contents.Invoke().ReadToEnd());

                    writer.Write(this.viewEngine.Render(templateContents, model, new NancyViewEngineHost(renderContext)));
                    writer.Flush();
                }
            };
        }
    }
}