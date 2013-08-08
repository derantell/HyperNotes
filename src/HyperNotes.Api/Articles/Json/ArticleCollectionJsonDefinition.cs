using System.Linq;
using HyperNotes.CollectionJson;

namespace HyperNotes.Api.Articles.Json {
    public class ArticleCollectionJsonDefinition : CollectionJsonDefinition<ArticleModel> {
        protected override CollectionBuilder<ArticleModel> GetBuilder() {
            return _builder;
        }

        private static readonly CollectionBuilder<ArticleModel> _builder =
            DefineCollection.For<ArticleModel>("/articles")

                .Items(
                    href: m => "/articles/" + m.Slug,
                    data: m => new[] {
                        Data(name: "title", value: m.Title, prompt: "Title"),
                        Data(name: "markdowntext", value: m.MarkdownText, prompt: "Markdown text"),
                        Data(name: "tags", value: string.Join(";", m.Tags), prompt: "Tags"),
                        Data(name: "created", value: m.Created.ToJavascriptDate(), prompt: "Created date"),
                        Data(name: "modified", value: m.Modified.ToJavascriptDate(), prompt: "Modified date"),
                        Data(name: "isprivate", value: m.IsPrivate, prompt: "Is private"),
                        Data(name: "iscollaborative", value: m.IsCollaborative, prompt: "Is collaborative")
                    },
                    links: m => m.Authors.Select(a => Link(rel: "author", href: "/users/" + a))
                )

                .Template(
                    Data(name: "title", value: "", prompt: "Title"),
                    Data(name: "markdowntext", value: "", prompt: "Markdown text"),
                    Data(name: "tags", value: "", prompt: "Tags"),
                    Data(name: "isprivate", value: "", prompt: "Is private"),
                    Data(name: "iscollaborative", value: "", prompt: "Is collaborative")
                );
    }
}