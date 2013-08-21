using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HyperNotes.Api.Notes;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace HyperNotes.Api.Persistance {
    public class Notes_NotesByTag : AbstractIndexCreationTask<Note> {
         public Notes_NotesByTag() {
             Map = notes => from note in notes
                            select new {note.Tags};

             Analyzers.Add( n => n.Tags, "SimpleAnalyzer");
             Indexes.Add( n => n.Tags,   FieldIndexing.Analyzed);
         }
    }

    public class Tags_TagsByName : AbstractIndexCreationTask<Note, Tags_TagsByName.Tag> {
        public class Tag { public string TagName { get; set; } }
        public Tags_TagsByName() {
            Map = notes => from note in notes
                           from tag in note.Tags
                           select new Tag {TagName = tag};

            Reduce = results => from result in results
                                group result by result.TagName into g
                                select new Tag{TagName = g.Key};
        }
    }

    public class Notes_NotesByText : AbstractIndexCreationTask<Note> {
        public Notes_NotesByText() {
            Map = notes => from note in notes
                           select new {note.Title, note.Tags, note.MarkdownText, note.Authors};

            Analyzers.Add(n => n.Title, "Lucene.Net.Analysis.Standard.StandardAnalyzer");
            Indexes  .Add( n => n.Title, FieldIndexing.Analyzed);

            Analyzers.Add( n => n.Tags, "SimpleAnalyzer");
            Indexes  .Add( n => n.Tags, FieldIndexing.Analyzed);

            Analyzers.Add(n => n.MarkdownText, "Lucene.Net.Analysis.Standard.StandardAnalyzer");
            Indexes  .Add( n => n.MarkdownText, FieldIndexing.Analyzed);

            Analyzers.Add( n => n.Authors, "SimpleAnalyzer");
            Indexes  .Add( n => n.Authors, FieldIndexing.Analyzed);
        }
    }
}