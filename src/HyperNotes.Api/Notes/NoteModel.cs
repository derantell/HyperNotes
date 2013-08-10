using System;
using System.Collections.Generic;

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
        public string CreatedRfc3339 { get { return Created.ToString("O"); } }
        public DateTime Modified { get; set; }
        public string ModifiedRfc3339 { get { return Modified.ToString("O"); } }
        public string Owner { get; set; }
    }
}