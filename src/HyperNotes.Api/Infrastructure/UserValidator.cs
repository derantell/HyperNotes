using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using HyperNotes.Api.Persistance;
using HyperNotes.Api.Users;
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

        public static bool IsValidUsername(string username) {
            return Regex.IsMatch(username ?? "", @"^[a-z0-9_-]{3,}$", RegexOptions.IgnoreCase);
        }

        public static bool IsValidEmailAddress(string emailAddress) {
            return Regex.IsMatch(emailAddress ?? "",
                                 @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}$",
                                 RegexOptions.IgnoreCase);
        }

        public static bool IsValidPassword(string password) {
            return password != null && password.Length >= 6;
        }

        public static bool IsValid(this UserDto self) {
            return IsValidUsername(self.UserName)
                   && IsValidEmailAddress(self.Email)
                   && IsValidPassword(self.Password);
        }
        
        public const string UserValidDataMessage = 
            "Username must be longer than 3 characters and the valid characters are " +
            "A-Z (case insensitive), 0-9, _ and -. Email address must be valid. " +
            "Password must be 6 characters or more.";
                    
    }
}