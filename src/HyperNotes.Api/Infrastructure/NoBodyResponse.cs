using System.Collections.Generic;
using Nancy;

namespace HyperNotes.Api.Infrastructure {
    public class NoBodyResponse : Response {
        public NoBodyResponse(
            HttpStatusCode status = HttpStatusCode.OK,
            IDictionary<string, string> headers = null) {

            StatusCode = status;
            ContentType = null;
            Contents = stream => { };

            if (headers != null) {
                Headers = headers;
            }
        }
    }
}