namespace EventManagementApi.Shared.Constants;

public abstract partial class Constants
{
    public static class Jwt
    {
        public const string DefaultKey = "this-is-a-fallback-and-it-does-not-matter";
        public const string DefaultIssuer = "EventManagementApi";
        public const int TokenExpirationHours = 1;
    }
}