using System;
using System.Security.Cryptography;
using System.Text;

namespace QuanApi.Utils
{
    public static class PasswordHasher
    {
        /// <summary>
        /// Hash mật khẩu sử dụng SHA256 với salt
        /// </summary>
        /// <param name="password">Mật khẩu gốc</param>
        /// <param name="salt">Salt (nếu null sẽ tự tạo)</param>
        /// <returns>Tuple chứa hash và salt</returns>
        public static (string hashedPassword, string salt) HashPassword(string password, string salt = null)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            // Tạo salt nếu chưa có
            if (string.IsNullOrEmpty(salt))
            {
                salt = GenerateSalt();
            }

            // Kết hợp password và salt
            string saltedPassword = password + salt;

            // Hash bằng SHA256
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                string hashedPassword = Convert.ToBase64String(hashedBytes);
                return (hashedPassword, salt);
            }
        }

        /// <summary>
        /// Xác thực mật khẩu
        /// </summary>
        /// <param name="password">Mật khẩu cần kiểm tra</param>
        /// <param name="hashedPassword">Mật khẩu đã hash trong database</param>
        /// <param name="salt">Salt từ database</param>
        /// <returns>True nếu mật khẩu đúng</returns>
        public static bool VerifyPassword(string password, string hashedPassword, string salt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(salt))
                return false;

            var (computedHash, _) = HashPassword(password, salt);
            return computedHash == hashedPassword;
        }

        /// <summary>
        /// Tạo salt ngẫu nhiên
        /// </summary>
        /// <returns>Salt string</returns>
        private static string GenerateSalt()
        {
            byte[] saltBytes = new byte[32]; // 256 bits
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Hash mật khẩu đơn giản (chỉ dùng cho testing, không khuyến khích)
        /// </summary>
        /// <param name="password">Mật khẩu gốc</param>
        /// <returns>Mật khẩu đã hash</returns>
        public static string SimpleHash(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "QuanAu_Salt_2024"));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// Xác thực mật khẩu đơn giản
        /// </summary>
        /// <param name="password">Mật khẩu cần kiểm tra</param>
        /// <param name="hashedPassword">Mật khẩu đã hash</param>
        /// <returns>True nếu mật khẩu đúng</returns>
        public static bool VerifySimpleHash(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            string computedHash = SimpleHash(password);
            return computedHash == hashedPassword;
        }
    }
}
