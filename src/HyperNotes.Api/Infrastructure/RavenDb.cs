using Raven.Client;
using Raven.Client.Document;

namespace HyperNotes.Api.Infrastructure {
    public static class RavenDb {
        public static IDocumentStore Store { get; private set; }

        public static void Init(string url) {
            Store = new DocumentStore {Url = url};
            Store.Initialize();
        }
    }
}