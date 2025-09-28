using System;
using System.Security.Cryptography;
using System.Text;

namespace QuanApi.Utils
{
    public static class PasswordGenerator
    {
        private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Numbers = "0123456789";
        private const string SpecialChars = "!@#$%^&*";

        /// <summary>
        /// Tạo mật khẩu ngẫu nhiên với độ dài và độ phức tạp được chỉ định
        /// </summary>
        /// <param name="length">Độ dài mật khẩu (tối thiểu 8 ký tự)</param>
        /// <param name="includeSpecialChars">Có bao gồm ký tự đặc biệt không</param>
        /// <returns>Mật khẩu được tạo ngẫu nhiên</returns>
        public static string GeneratePassword(int length = 12, bool includeSpecialChars = true)
        {
            if (length < 8)
                throw new ArgumentException("Độ dài mật khẩu phải ít nhất 8 ký tự", nameof(length));

            var charset = LowerCase + UpperCase + Numbers;
            if (includeSpecialChars)
                charset += SpecialChars;

            var password = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                // Đảm bảo mật khẩu có ít nhất một ký tự từ mỗi loại
                password.Append(GetRandomChar(LowerCase, rng));
                password.Append(GetRandomChar(UpperCase, rng));
                password.Append(GetRandomChar(Numbers, rng));
                
                if (includeSpecialChars)
                    password.Append(GetRandomChar(SpecialChars, rng));

                // Điền phần còn lại với ký tự ngẫu nhiên
                for (int i = password.Length; i < length; i++)
                {
                    password.Append(GetRandomChar(charset, rng));
                }
            }

            // Trộn các ký tự để tránh pattern cố định
            return ShuffleString(password.ToString());
        }

        /// <summary>
        /// Lấy một ký tự ngẫu nhiên từ chuỗi ký tự cho trước
        /// </summary>
        private static char GetRandomChar(string chars, RandomNumberGenerator rng)
        {
            byte[] randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            int randomIndex = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % chars.Length;
            return chars[randomIndex];
        }

        /// <summary>
        /// Trộn các ký tự trong chuỗi
        /// </summary>
        private static string ShuffleString(string input)
        {
            char[] array = input.ToCharArray();
            using (var rng = RandomNumberGenerator.Create())
            {
                for (int i = array.Length - 1; i > 0; i--)
                {
                    byte[] randomBytes = new byte[4];
                    rng.GetBytes(randomBytes);
                    int randomIndex = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % (i + 1);
                    
                    // Swap
                    char temp = array[i];
                    array[i] = array[randomIndex];
                    array[randomIndex] = temp;
                }
            }
            return new string(array);
        }

        /// <summary>
        /// Tạo mật khẩu đơn giản dễ nhớ (chỉ chữ và số)
        /// </summary>
        /// <param name="length">Độ dài mật khẩu</param>
        /// <returns>Mật khẩu đơn giản</returns>
        public static string GenerateSimplePassword(int length = 10)
        {
            return GeneratePassword(length, includeSpecialChars: false);
        }
    }
}
