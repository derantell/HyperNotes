using HyperNotes.Api.CollectionJson;

namespace HyperNotes.Api.Users.Json {
    public class UserCollectionDefinition : CollectionJsonDefinition<UserModel> {

        protected override CollectionBuilder<UserModel> GetBuilder() {
            return Builder;
        }

        private static readonly CollectionBuilder<UserModel> Builder =

            DefineCollection.For<UserModel>(href: "/users")

               .Links( 
                   Link(rel: "alternate", href: "/users.html"),
                   Link(rel: "feed", href: "/users.atom")
               )

               .Items(
                   href: u => "/users/" + u.UserName,
                   data: u => new[] {
                       Data( name: "name", prompt: "Name", value: u.UserName),
                       Data( name: "email", prompt: "Email", value: u.Email),
                   }
               )

               .Template(
                   Data(name: "name", prompt: "Name", value: ""),
                   Data(name: "email", prompt: "Email address", value: ""),
                   Data(name: "password", prompt: "Password", value: "")
               );
    }
}