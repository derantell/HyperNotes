using System;
using System.Collections.Generic;
using System.Globalization;

namespace HyperNotes.Api.Notes {
    public class NoteModel {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string MarkdownText { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsCollaborative { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string Owner { get; set; }
    }

    public class NoteViewModel {
        public NoteViewModel(NoteModel note) {
            _note = note;
        }

        public NoteModel Note { get { return _note; }}

        public string CreatedRfc3339 { get { return Note.Created.ToString("O"); } }
        public string ModifiedRfc3339 { get { return Note.Modified.ToString("O"); } }

        public string CreatedJs { get { return Note.Created.ToJavascriptDate().ToString( CultureInfo.InvariantCulture ); } }
        public string ModifiedJs { get { return Note.Modified.ToJavascriptDate().ToString(CultureInfo.InvariantCulture); } }

        public string IsCollaborativeJs { get { return Note.IsCollaborative.ToString().ToLower(); } }
        public string IsPrivateJs { get { return Note.IsPrivate.ToString().ToLower(); } }

        public string TagList { get { return string.Join(" ", Note.Tags); } }

        private readonly NoteModel _note;
    }
}