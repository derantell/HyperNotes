using Raven.Client;
using Raven.Client.Document;

namespace HyperNotes.Api.Infrastructure {
    public static class Raven {
        public static IDocumentStore Db { get; private set; }

        public static void Init(string url) {
            Db = new DocumentStore {Url = url};
            Db.Initialize();
        }
    }
}