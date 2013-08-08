using HyperNotes.CollectionJson;

namespace HyperNotes.Api.Errors.Json {
    public class ErrorCollectionDefinition : CollectionJsonDefinition<ErrorModel> {
        protected override CollectionBuilder<ErrorModel> GetBuilder() {
            return _builder;
        }


        private static readonly CollectionBuilder<ErrorModel> _builder =
            DefineCollection.For<ErrorModel>("/users")

                .Error(error => new Error {
                    title = error.Title,
                    code = error.Code,
                    message = error.Message
                });
    }
}