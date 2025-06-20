namespace EventManagementApi.Shared.Constants
{
    public static class Jwt
    {
        public const string DefaultKey = "this-is-a-fallback-and-it-does-not-matter";
        public const string DefaultIssuer = "EventManagementApi";
        public const string BearerScheme = "Bearer";
        public const int TokenExpirationHours = 1;
    }
} 