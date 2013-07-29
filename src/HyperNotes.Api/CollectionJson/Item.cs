using System.Collections.Generic;

namespace HyperNotes.Api.CollectionJson {
    public class Item {
        public string href { get; set; }
        public IEnumerable<Link> links { get; set; }
        public IEnumerable<Data> data { get; set; }   
    }
}