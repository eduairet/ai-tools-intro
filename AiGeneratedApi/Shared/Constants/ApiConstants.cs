namespace EventManagementApi.Shared.Constants;

public abstract partial class Constants
{
    public static class Api
    {
        private const string ApiVersion = "v1";

        public static class Routes
        {
            private const string ApiBase = $"api/{ApiVersion}";
            public const string Users = $"{ApiBase}/users";
            public const string Events = $"{ApiBase}/events";
            public const string EventsRegistrations = $"{Events}/{{eventId}}/register";
            public const string RegistrationsAdmin = $"{ApiBase}/events-registrations";
        }

        public static class ErrorMessages
        {
            public const string InvalidCredentials = "Invalid email or password.";
            public const string UserNotFound = "User not found.";
            public const string UserAlreadyExists = "User with this email already exists.";
            public const string InvalidToken = "Invalid token.";

            public const string AuthenticationFailure =
                "User not found, refresh token mismatch, or refresh token expired.";

            public static string TokenValidationFailed(string message) => $"Token validation failed: {message}";
            public const string UnauthorizedAccess = "You can only access resources you own.";
            public const string OnlyImagesAllowed = "Invalid file type. Only images are allowed.";
            public const string EventNotFound = "Event not found.";
            public const string RegistrationNotFound = "Registration not found.";
            public const string CannotRegisterForOwnEvent = "You cannot register for your own event.";
            public const string AlreadyRegisteredForEvent = "You are already registered for this event.";
            public const string InvalidId = "The provided ID is not a valid format.";
        }

        public static class FileUpload
        {
            public const string TempFolder = "temp";
            public const string AllowedExtensions = ".jpg,.jpeg,.png,.gif";
            public const int MaxFileSizeMb = 10;
        }
    }
}