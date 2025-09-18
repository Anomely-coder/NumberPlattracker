using CarMaintenance.Data;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace CarMaintenance.Helpers
{
    public static class PasswordHelper
    {
        private static readonly string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string Digits = "0123456789";
        private static readonly string SpecialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
        private static readonly string AllChars = Uppercase + Lowercase + Digits + SpecialChars;

        public static bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                Console.WriteLine("Password validation failed: Length < 8 or empty");
                return false;
            }
            if (!password.Any(char.IsUpper))
            {
                Console.WriteLine("Password validation failed: No uppercase letter");
                return false;
            }
            if (!password.Any(char.IsLower))
            {
                Console.WriteLine("Password validation failed: No lowercase letter");
                return false;
            }
            if (!password.Any(char.IsDigit))
            {
                Console.WriteLine("Password validation failed: No digit");
                return false;
            }
            if (!password.Any(ch => SpecialChars.Contains(ch)))
            {
                Console.WriteLine("Password validation failed: No special character");
                return false;
            }
            return true;
        }

        public static string GeneratePassword(int length = 16)
        {
            if (length < 8)
                throw new ArgumentException("Password length must be at least 8 characters.");

            string password = "";
            var rng = RandomNumberGenerator.Create();
            byte[] randomBytes = new byte[1];

            password += GetRandomChar(Uppercase, rng);
            password += GetRandomChar(Lowercase, rng);
            password += GetRandomChar(Digits, rng);
            password += GetRandomChar(SpecialChars, rng);

            for (int i = 4; i < length; i++)
            {
                password += GetRandomChar(AllChars, rng);
            }

            char[] passwordArray = password.ToCharArray();
            for (int i = passwordArray.Length - 1; i > 0; i--)
            {
                rng.GetBytes(randomBytes);
                int j = randomBytes[0] % (i + 1);
                (passwordArray[i], passwordArray[j]) = (passwordArray[j], passwordArray[i]);
            }

            string finalPassword = new string(passwordArray);
            Console.WriteLine($"Generated Password: {finalPassword}");
            return finalPassword;
        }

        public static bool IsPasswordInHistory(string password, int userId, AppDbContext db)
        {
            var recentPasswords = db.Tbl_PasswordHistory
                .Where(ph => ph.UserID == userId)
                .OrderByDescending(ph => ph.CreatedAt)
                .Take(2)
                .Select(ph => ph.Password)
                .ToList();

            bool isInHistory = recentPasswords.Contains(password);
            if (isInHistory)
                Console.WriteLine($"Password validation failed: Password matches one of the last two used for UserID {userId}");
            return isInHistory;
        }

        private static char GetRandomChar(string charSet, RandomNumberGenerator rng)
        {
            byte[] randomBytes = new byte[1];
            rng.GetBytes(randomBytes);
            int index = randomBytes[0] % charSet.Length;
            return charSet[index];
        }
    }
}