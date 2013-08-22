using System;
using HyperNotes.Api.Notes;
using HyperNotes.Api.Persistance;
using Nancy;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Indexes;
using System.Linq;

namespace HyperNotes.Api.Tags {
    public class TagModule : NancyModule {
        public TagModule() : base("/tags") {
            Get["/"] = _ => {
                using (var db = RavenDb.Store.OpenSession())
                {
                    var tags = db.Query<Tags_TagsByName.Tag, Tags_TagsByName>();

                    return Negotiate
                        .WithModel(new FunctionalList<Tags_TagsByName.Tag>(tags))
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithView("Tags/Representations/AllTags");
                }
            };

            Get["/{tags}"] = param => {
                var tags = (string)param.tags;
                var terms = string.Join(" ", tags.SplitList());

                using (var db = RavenDb.Store.OpenSession()) {
                    var matches = db.Advanced.LuceneQuery<Note>("Notes/NotesByTag")
                        .UsingDefaultOperator(QueryOperator.And)
                        .UsingDefaultField("Tags")
                        .Where(terms);

                    var notes = new FunctionalList<Note>(matches);
                    return Negotiate
                        .WithModel(new {Tags = param.tags, Notes = notes})
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithView("Tags/Representations/NotesByTag");
                }
            };
        }
    }
}