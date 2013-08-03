using System.Collections.Generic;

namespace HyperNotes.CollectionJson {
    public class Collection {
        public string version { get; set; }
        public string href { get; set; }
        public IEnumerable<Link> links { get; set; }
        public IEnumerable<Item> items { get; set; }
        public IEnumerable<Query> queries { get; set; }
        public Template template { get; set; }
        public Error error { get; set; }
    }
}