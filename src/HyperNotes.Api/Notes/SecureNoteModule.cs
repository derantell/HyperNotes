using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HyperNotes.Api.Infrastructure;
using HyperNotes.Api.Persistance;
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
                var mappedNote = Mapper.Map<NoteDto, Note>(postedNoteData);
                var nowUTC = DateTime.UtcNow;

                using (var db = RavenDb.Store.OpenSession()) {
                    mappedNote.Created  = nowUTC;
                    mappedNote.Modified = nowUTC;
                    mappedNote.Owner    = Context.CurrentUser.UserName;
                    mappedNote.Authors  = new[] {mappedNote.Owner};
                    mappedNote.Slug     = db.GetUniqueSlug( mappedNote.Title );
                    mappedNote.Tags     = mappedNote.Tags.Select(t => t.ToLower());

                    db.Store(mappedNote);
                    db.SaveChanges();
                    
                    return new Response {
                        StatusCode = HttpStatusCode.Created,
                        ContentType = null,
                        Headers = new Dictionary<string, string> {
                            {"Location", Context.GetAbsoluteUrl( "notes", mappedNote.Slug) }
                        }
                    };
                }
            };

            Delete["/{slug}"] = param => {
                var slug = (string) param.slug;

                using (var db = RavenDb.Store.OpenSession()) {
                    var note = db.FindNote(slug);

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

            Put["/{slug}"] = param => {
                var slug = (string) param.slug;

                using (var db = RavenDb.Store.OpenSession()) {
                    var note = db.FindNote(slug);
                    
                    if (note == null) {
                        return Negotiate.WithError(HttpStatusCode.NotFound, "No such note");
                    }

                    var isOwner = UserValidationHelper.IsLoggedInUser(Context.CurrentUser.UserName, note.Owner);
                    if (!isOwner && note.IsPrivate) {
                        return Negotiate.WithError(HttpStatusCode.NotFound, "No such note");
                    }

                    if (!isOwner && !note.IsCollaborative) {
                        return Negotiate.WithError(HttpStatusCode.Forbidden, "Note is not collaborative");
                    }

                    var postedNoteData = this.Bind<NoteDto>();
                    var mappedNote = Mapper.Map<NoteDto, Note>(postedNoteData);

                    note.Title = mappedNote.Title;
                    note.Tags = mappedNote.Tags;
                    note.MarkdownText = mappedNote.MarkdownText;
                    note.Modified = DateTime.UtcNow;

                    if (isOwner) {
                        note.IsPrivate = mappedNote.IsPrivate;
                        note.IsCollaborative = mappedNote.IsCollaborative;
                    }

                    if (!note.Authors.Contains(Context.CurrentUser.UserName)) {
                        note.Authors = note.Authors.Concat(new[] {Context.CurrentUser.UserName});
                    }

                    db.Store(note);
                    db.SaveChanges();

                    return Negotiate
                        .WithModel(new NoteViewModel(note))
                        .WithHeaders(db.GetCacheHeaders(note))
                        .WithView("Notes/Representations/Single");
                }
            };
        }
    }
    
    public class ReadNoteModule : NancyModule {
        public ReadNoteModule() : base("/notes") {

            Get["/"] = param => {
                using (var db = RavenDb.Store.OpenSession()) {
                    var notes = db.Query<Note>().ToArray();

                    return Negotiate
                        .WithModel( new FunctionalList<NoteViewModel>( notes.Select( m => new NoteViewModel(m)) ))
                        .WithView("Notes/Representations/List");
                }
            };

            Get["/{slug}"] = param => {
                var slug = (string) param.slug;

                using (var db = RavenDb.Store.OpenSession()) {
                    var note = db.FindNote(slug);

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
}
