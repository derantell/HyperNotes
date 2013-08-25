using System.Linq;
using Nancy;

namespace HyperNotes.Api {
    public static class NancyContextExtensions {
        public static string GetAbsoluteUrl(this NancyContext self, params string[] segments ) {
            var baseUrl = self.Request.Url.SiteBase;

            if (segments == null || segments.Length == 0) {
                return baseUrl;
            }

            var url = string.Join("/", new[] {baseUrl}.Concat(segments));

            return url;
        } 
    }
}