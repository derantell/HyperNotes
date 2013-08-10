using HyperNotes.Api.Errors;
using Nancy;
using Nancy.Responses.Negotiation;

namespace HyperNotes.Api {
    public static class NegotiatorExtensions {
         public static Negotiator WithError(this Negotiator self, 
             HttpStatusCode statusCode = HttpStatusCode.InternalServerError, 
             string title = "Server error",
             string message = "") {
             var url = self.NegotiationContext.ModulePath;
             return self
                 .WithStatusCode(statusCode)
                 .WithModel(new ErrorModel {
                     Code = ((int)statusCode).ToString(),
                     Title = title,
                     Message = message,
                     CollectionUrl = url
                 })
                 .WithView("Errors/Representations/Error");
         }
    }
}