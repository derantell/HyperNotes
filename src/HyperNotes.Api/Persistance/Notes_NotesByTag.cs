using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HyperNotes.Api.Notes;
using Raven.Client.Indexes;

namespace HyperNotes.Api.Persistance {
    public class Notes_NotesByTag : AbstractIndexCreationTask<Note> {
         public Notes_NotesByTag() {
             Map = notes => from note in notes
                            select new {note.Tags};

             Analyzers.Add( n => n.Tags, "SimpleAnalyzer");
         }
    }
}