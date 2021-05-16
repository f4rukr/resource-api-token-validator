using System;
using System.Collections.Generic;
using System.Text;

namespace ADB2C.Model.Constants
{
    public static class AuthorizationErrorMessages
    {
        public static class TokenNotProvided
        {
            public const string UserMessage = "Unauthorized request";
            public const string DeveloperMessage = "Token is not provided";
        }

        public static class TokenInvalid
        {
            public const string UserMessage = "Unauthorized request";
            public const string DeveloperMessage = "Provided token is not correct";
        }

        public static class TokenExpired
        {
            public const string UserMessage = "Unauthorized request";
            public const string DeveloperMessage = "Provided token has expired";
        }

        public static class UnverifiedClaim
        {
            public const string UserMessage = "Unauthorized request";
            public const string DeveloperMessage = "Provided token has insufficient permissions for this operation";
        }
    }
}
