using BCrypt.Net;

namespace EventManagementApi.Shared.Helpers;

public static partial class Helpers
{
    public static class Password
    {
        public static string Hash(string password)
        {
            return string.IsNullOrEmpty(password) ? string.Empty : BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool Verify(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch (SaltParseException)
            {
                return false;
            }
        }
    }
}