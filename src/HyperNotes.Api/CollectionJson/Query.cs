using System.Collections.Generic;

namespace HyperNotes.Api.CollectionJson {
    public class Query {
        public string rel { get; set; }
        public string href { get; set; }
        public string prompt { get; set; }
        public string name { get; set; }
        public IEnumerable<Data> data { get; set; }
    }
}