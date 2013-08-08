using System;
using System.Text.RegularExpressions;
using AutoMapper;
using HyperNotes.Api.Infrastructure;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace HyperNotes.Api.Articles {
    public class SecureArticleModule : NancyModule {
        public SecureArticleModule() : base("/articles") {
            this.RequiresAuthentication();

            Post["/"] = param => {
                var postedArticleData = this.Bind<ArticleDto>();
                var mappedArticle = Mapper.Map<ArticleDto, ArticleModel>(postedArticleData);
                var nowUTC = DateTime.UtcNow;

                using (var db = RavenDb.Store.OpenSession()) {
                    mappedArticle.Created = nowUTC;
                    mappedArticle.Modified = nowUTC;
                    mappedArticle.Owner = Context.CurrentUser.UserName;
                    mappedArticle.Authors = new[] {mappedArticle.Owner};
                    mappedArticle.Slug = mappedArticle.Title.Slugify();

                    db.Store(mappedArticle);
                    db.SaveChanges();

                    return Negotiate
                        .WithStatusCode(HttpStatusCode.Created)
                        .WithContentType(null)
                        .WithHeader( "Location", "/articles/" + mappedArticle.Slug)
                        .WithHeaders(db.GetCacheHeaders(mappedArticle));
                }
            };
        }
    }
    
    public class ReadArticleModule : NancyModule {
        public ReadArticleModule() : base("/articles") {

            Get["/{slug}"] = param => {
                var slug = (string) param.slug;

                using (var db = RavenDb.Store.OpenSession()) {
                    var article = db.FindArticle(slug);

                    if (article == null) {
                        return Negotiate.WithError(HttpStatusCode.NotFound, "No such article");
                    }

                    return Negotiate
                        .WithModel(article)
                        .WithHeaders(db.GetCacheHeaders(article))
                        .WithView("Articles/Html/Article");
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
