using System.Text.RegularExpressions;

namespace HyperNotes.Api {
    public static class StringExtensions {

        public static string[] SplitList(this string self, string separators = " ,;") {
            return Regex.Split(self, "[" + separators + "]+");
        }

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