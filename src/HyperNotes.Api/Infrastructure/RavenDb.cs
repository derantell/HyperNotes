using System;
using System.Linq;
using HyperNotes.Api.Notes;
using HyperNotes.Api.Users;
using Raven.Client;
using Raven.Client.Document;

namespace HyperNotes.Api.Infrastructure {
    public static class RavenDb {
        public static IDocumentStore Store { get; private set; }

        public static void Init(string url) {
            Store = new DocumentStore {Url = url};
            Store.Initialize();
        }
    }

    public static class DocumentSessionExtensions {
        public static UserModel FindUser(this IDocumentSession self, string username) {
            // TODO: Use index when we know how to do that
            return self.Query<UserModel>().FirstOrDefault(u => u.UserName.Equals(username));
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

        public static NoteModel FindNote(this IDocumentSession self, string slug) {
            return self.Query<NoteModel>().FirstOrDefault(u => u.Slug.Equals(slug));
        }
    }
}