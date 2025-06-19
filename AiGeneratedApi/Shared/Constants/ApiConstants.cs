namespace EventManagementApi.Shared.Constants
{
    public static class Constants
    {
        public static class ApiConstants
        {
            public static class Routes
            {
                public const string ApiBase = "api";
                public const string Users = "users";
                public const string Events = "events";
                public const string EventsRegistration = "eventsregistration";
            }
            public static class ErrorMessages
            {
                public const string UserNotFound = "User not found.";
                public const string InvalidCredentials = "Invalid email or password.";
                public const string UserAlreadyExists = "User with this email already exists.";
                public const string InvalidToken = "Invalid token.";
                public const string TokenExpired = "Token has expired.";
                public const string UnauthorizedAccess = "You can only access resources you own.";
            }
            public static class FileUpload
            {
                public const string TempFolder = "temp";
                public const string AllowedExtensions = ".jpg,.jpeg,.png,.gif";
                public const int MaxFileSizeMB = 10;
            }
        }
    }
} 