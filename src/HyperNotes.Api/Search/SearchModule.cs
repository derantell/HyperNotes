using HyperNotes.Api.Notes;
using HyperNotes.Api.Persistance;
using Nancy;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace HyperNotes.Api.Search {
    public class SearchModule : NancyModule {
        public SearchModule() : base("/search") {
            Get["/tag/{tag}"] = param => {
                var tag = (string)param.tag;

                using (var db = RavenDb.Store.OpenSession()) {
                    var matches = db.Query<Note>("Notes/NotesByTag")
                        .Where( n => n.Tags.Contains( tag ) );

                    return Negotiate
                        .WithModel(matches)
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithView("Search/Representations/List");
                }
            };
        }

        
    }

    public class SearchParameters {
        public string Query { get; set; }
        public string Tags { get; set; }
        public string Users { get; set; }

        public string[] TagList { get { return Tags.SplitList(); } }
        public string[] UserList { get { return Users.SplitList(); } }

        public SearchParameters() {
            Query = Tags = Users = "";
        }
    }
}