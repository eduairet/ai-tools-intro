namespace EventManagementApi.Shared.Constants
{
    public static class Jwt
    {
        public const string DefaultSecretKey = "";
        public const string DefaultIssuer = "EventManagementApi";
        public const string BearerScheme = "Bearer";
        public const int TokenExpirationHours = 1;
    }
} 