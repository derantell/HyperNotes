using System.Collections.Generic;
using Nancy.Security;

namespace HyperNotes.Api.Users {
    public class User : IUserIdentity {
        public string Id { get; set; }
        public string Email { get; set; }
        public string PwHash { get; set; }
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}