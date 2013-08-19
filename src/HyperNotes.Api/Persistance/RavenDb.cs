using System;
using System.Linq;
using HyperNotes.Api.Notes;
using HyperNotes.Api.Users;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace HyperNotes.Api.Persistance {
    public static class RavenDb {
        public static IDocumentStore Store { get; private set; }

        public static void Init(string url) {
            Store = new DocumentStore {Url = url};
            Store.Initialize();

            IndexCreation.CreateIndexes(typeof(RavenDb).Assembly, Store);
        }
    }

    public static class DocumentSessionExtensions {
        public static User FindUser(this IDocumentSession self, string username) {
            return self.Query<User>().FirstOrDefault(u => u.UserName.Equals(username));
        } 

        public static object[] GetCacheHeaders<TModel>(this IDocumentSession self, TModel model) {
            var metadata = self.Advanced.GetMetadataFor(model);
            var modified = metadata.Value<DateTime>("Last-Modified");
            var etag = metadata.Value<string>("@etag");

            return new object[] {
                new { header = "ETag", value = etag },
                new { header = "Last-Modified", value = modified.ToString("r") }
            };
        }

        public static Note FindNote(this IDocumentSession self, string slug) {
            return self.Query<Note>().FirstOrDefault(u => u.Slug.Equals(slug));
        }
    }
}