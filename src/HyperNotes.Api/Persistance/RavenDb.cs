using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace HyperNotes.Api.Persistance {
    public static class RavenDb {
        public static IDocumentStore Store { get; private set; }

        public static void Init(string url) {
            Store = new DocumentStore {Url = url,DefaultDatabase = "HyperNotes"};
            Store.Initialize();

            IndexCreation.CreateIndexes(typeof(RavenDb).Assembly, Store);
        }
    }
}