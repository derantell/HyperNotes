using System.Linq;
using HyperNotes.Api.Notes;
using HyperNotes.Api.Persistance;
using Nancy;
using Nancy.ModelBinding;
using Raven.Client;
using Raven.Client.Linq;

namespace HyperNotes.Api.Search {
    public class SearchModule : NancyModule {
         public SearchModule() : base("/search") {
             Get["/"] = param => {
                 var query = this.Bind<SearchParams>();

                 using (var db = RavenDb.Store.OpenSession()) {
                     var matches = db.Query<Note>("Notes/NotesByText")
                                     .Search(n => n.Tags, query.Q, boost: 10)
                                     .Search(n => n.Authors, query.Q, boost: 10)
                                     .Search(n => n.Title, query.Q, boost: 5)
                                     .Search(n => n.MarkdownText, query.Q);
                     
                     if (query.T != "") {
                         matches = matches.Where(n => n.Tags.Contains(query.T));
                     }

                     if (query.U != "") {
                         matches = matches.Where(n => n.Authors.Contains(query.U) );
                     }

                     return Negotiate
                         .WithModel( new { Matches = new FunctionalList<Note>(matches), SearchParams = query} )
                         .WithView("Search/Representations/Result")
                         .WithStatusCode(HttpStatusCode.OK);
                 }
             };
         }


        public class SearchParams {
            public string Q { get; set; }
            public string T { get; set; }
            public string U { get; set; }

            public string[] Tags { get { return T.SplitList(); } }
            public string[] Users { get { return U.SplitList(); } }

            public SearchParams() {
                Q = T = U = "";
            }
        }
    }
}