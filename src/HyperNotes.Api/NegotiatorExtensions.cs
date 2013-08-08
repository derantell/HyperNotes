using HyperNotes.Api.Errors;
using Nancy;
using Nancy.Responses.Negotiation;

namespace HyperNotes.Api {
    public static class NegotiatorExtensions {
         public static Negotiator WithError(this Negotiator self, 
             HttpStatusCode statusCode = HttpStatusCode.InternalServerError, 
             string title = "Server error",
             string message = "") {
             return self
                 .WithStatusCode(statusCode)
                 .WithModel(new ErrorModel {Code = ((int)statusCode).ToString(), Title = title, Message = message})
                 .WithView("Errors/Html/Error");
         }
    }
}