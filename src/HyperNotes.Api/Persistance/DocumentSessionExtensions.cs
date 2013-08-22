using System;
using System.Linq;
using HyperNotes.Api.Notes;
using HyperNotes.Api.Users;
using Raven.Client;

namespace HyperNotes.Api.Persistance {
    public static class DocumentSessionExtensions {
        public static User FindUser(this IDocumentSession self, string username) {
            return self.Query<User>().FirstOrDefault(u => u.UserName.Equals(username));
        } 

        public static object[] GetCacheHeaders<TModel>(this IDocumentSession self, TModel model) {
            var metadata = self.Advanced.GetMetadataFor(model);
            var modified = metadata.Value<DateTime>("Last-Modified");
            var etag = metadata.Value<string>("@etag");

            return new object[] {
                new { header = "ETag",          value = etag },
                new { header = "Last-Modified", value = modified.ToString("r") }
            };
        }

        public static Note FindNote(this IDocumentSession self, string slug) {
            return self.Query<Note>().FirstOrDefault(u => u.Slug.Equals(slug));
        }


        public static string GetUniqueSlug( this IDocumentSession self, string text) {
            var index = 0;
            var slug = text.Slugify();
            do {
                var note = self.Query<Note>().FirstOrDefault(n => n.Slug == slug);
                if (note == null) {
                    return slug;
                }
                slug = (text + " " + ++index).Slugify();
            } while (true);

        }
    }
}