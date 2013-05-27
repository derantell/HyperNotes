namespace HyperNotes.Api.Users {
    public class UserModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PwHash { get; set; }
    }
}