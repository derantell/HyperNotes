using System;
using System.Security.Cryptography;
using System.Text;
using HyperNotes.Api.Persistance;
using Nancy.Authentication.Basic;
using Nancy.Security;

namespace HyperNotes.Api.Infrastructure {
    public class UserValidator : IUserValidator {
        public IUserIdentity Validate(string username, string password) {
             using (var db = RavenDb.Store.OpenSession()) {
                 var user = db.FindUser(username);

                 if (user != null && UserValidationHelper.IsValidHash(password, user.PwHash)) {
                     return user;
                 }

                 return null;
             }
        }
    }
    
    public static class UserValidationHelper {
        public static bool IsValidHash(string password, string hash) {
            return GetHash(password) == hash;
        }

        // Better hashing: https://crackstation.net/hashing-security.htm
        public static string GetHash(string password) {
            using (var sha256 = SHA256.Create()) {
                var passwordBuffer = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(passwordBuffer);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool IsLoggedInUser(string loggedInUser, string user) {
            return loggedInUser == user;
        }
    }
}