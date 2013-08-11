using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using HyperNotes.Api.Infrastructure;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using Nancy.Security;

namespace HyperNotes.Api.Notes {
    public class SecureNoteModule : NancyModule {
        public SecureNoteModule() : base("/notes") {
            this.RequiresAuthentication();

            Post["/"] = param => {
                var postedNoteData = this.Bind<NoteDto>();
                var mappedNote = Mapper.Map<NoteDto, NoteModel>(postedNoteData);
                var nowUTC = DateTime.UtcNow;

                using (var db = RavenDb.Store.OpenSession()) {
                    mappedNote.Created = nowUTC;
                    mappedNote.Modified = nowUTC;
                    mappedNote.Owner = Context.CurrentUser.UserName;
                    mappedNote.Authors = new[] {mappedNote.Owner};
                    mappedNote.Slug = mappedNote.Title.Slugify();

                    db.Store(mappedNote);
                    db.SaveChanges();

                    return new Response {
                        StatusCode = HttpStatusCode.Created,
                        ContentType = null,
                        Headers = new Dictionary<string, string> {
                            {"Location", "/notes/" + mappedNote.Slug}
                        }
                    };
                }
            };

            Delete["/{slug}"] = param => {
                var slug = (string) param.slug;

                using (var db = RavenDb.Store.OpenSession()) {
                    var note = db.FindArticle(slug);

                    if (note == null) {
                        return new NoBodyResponse();
                    }

                    if (!UserValidationHelper.IsLoggedInUser(Context.CurrentUser.UserName, note.Owner)) {
                        return Negotiate.WithError(HttpStatusCode.Forbidden, "Cannot delete other user's note");
                    }

                    db.Delete(note);
                    db.SaveChanges();

                    return new NoBodyResponse();
                }
            };
            
        }
    }
    
    public class ReadNoteModule : NancyModule {
        public ReadNoteModule() : base("/notes") {

            Get["/"] = param => {
                using (var db = RavenDb.Store.OpenSession()) {
                    var notes = db.Query<NoteModel>().ToArray();

                    return Negotiate
                        .WithModel( new FunctionalList<NoteViewModel>( notes.Select( m => new NoteViewModel(m)) ))
                        .WithView("Notes/Representations/List");
                }
            };

            Get["/{slug}"] = param => {
                var slug = (string) param.slug;

                using (var db = RavenDb.Store.OpenSession()) {
                    var note = db.FindArticle(slug);

                    if (note == null) {
                        return Negotiate.WithError(HttpStatusCode.NotFound, "No such note");
                    }

                    return Negotiate
                        .WithModel(new NoteViewModel(note))
                        .WithHeaders(db.GetCacheHeaders(note))
                        .WithView("Notes/Representations/Single");
                }
            };
        }
    }

    public static class StringExtensions {

        public static string Slugify(this string phrase) {
            var slug = RemoveDiacritics(phrase).ToLower();

            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", ""); // invalid chars           
            slug = Regex.Replace(slug, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            slug = slug.Substring(0, slug.Length <= 45 ? slug.Length : 45).Trim(); // cut and trim it   
            slug = Regex.Replace(slug, @"\s", "-"); // hyphens   

            return slug; 
        }

        private static string RemoveDiacritics(string txt) {
            var bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}
