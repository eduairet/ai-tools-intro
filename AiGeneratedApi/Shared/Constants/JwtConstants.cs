namespace EventManagementApi.Shared.Constants;

public abstract partial class Constants
{
    public static class Jwt
    {
        public const string DefaultAudience = "EventManagementApiAudience";
        public const string DefaultIssuer = "EventManagementApi";
        public const string DefaultKey = "this-is-a-fallback-and-it-does-not-matter";
        public const int TokenExpirationMinutes = 60;
    }
}