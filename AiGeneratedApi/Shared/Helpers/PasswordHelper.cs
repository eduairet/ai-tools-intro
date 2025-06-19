using System.Text;

namespace EventManagementApi.Shared.Helpers
{
    public static partial class Helpers
    {
        public static class Password
        {
            public static string Hash(string password)
            {
                if (string.IsNullOrEmpty(password))
                    return string.Empty;
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
            }
            public static bool Verify(string password, string hash)
            {
                if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                    return false;
                var hashedPassword = Hash(password);
                return hash == hashedPassword;
            }
        }
    }
} 