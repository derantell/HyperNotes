﻿namespace HyperNotes.Api.Articles {
    public class ArticleDto {
        public string Title { get; set; }
        public string Tags { get; set; }
        public string MarkdownText { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsCollaborative { get; set; }
    }
}