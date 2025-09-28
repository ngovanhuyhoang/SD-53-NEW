using Microsoft.EntityFrameworkCore;
using QuanApi.Data;
using QuanApi.Utils;
using BanQuanAu1.Web.Data;
using Microsoft.Extensions.Logging;

namespace QuanApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly BanQuanAu1DbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(BanQuanAu1DbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Xác thực đăng nhập bằng email và mật khẩu
        /// </summary>
        /// <param name="email">Email đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Thông tin nhân viên nếu xác thực thành công, null nếu thất bại</returns>
        public async Task<NhanVien?> AuthenticateAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("Authentication failed: Email or password is empty");
                    return null;
                }

                // Tìm nhân viên theo email
                var nhanVien = await _context.NhanViens
                    .Include(n => n.VaiTro)
                    .FirstOrDefaultAsync(n => n.Email == email && n.TrangThai == true);

                if (nhanVien == null)
                {
                    _logger.LogWarning("Authentication failed: No active employee found with email {Email}", email);
                    return null;
                }

                // Kiểm tra mật khẩu plain text
                bool isPasswordValid = (nhanVien.MatKhau == password);
                
                if (isPasswordValid)
                {
                    _logger.LogInformation("Password verification successful for employee {Email}", email);
                }
                else
                {
                    _logger.LogWarning("Password verification failed for employee {Email}", email);
                }

                if (isPasswordValid)
                {
                    _logger.LogInformation("Authentication successful for employee {Email} ({Id})", email, nhanVien.IDNhanVien);
                    return nhanVien;
                }
                else
                {
                    _logger.LogWarning("Authentication failed: Invalid password for employee {Email}", email);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for email {Email}", email);
                return null;
            }
        }

        /// <summary>
        /// Xác thực mật khẩu cho một nhân viên cụ thể
        /// </summary>
        /// <param name="employeeId">ID nhân viên</param>
        /// <param name="password">Mật khẩu cần kiểm tra</param>
        /// <returns>True nếu mật khẩu đúng</returns>
        public async Task<bool> ValidatePasswordAsync(Guid employeeId, string password)
        {
            try
            {
                var nhanVien = await _context.NhanViens.FindAsync(employeeId);
                if (nhanVien == null || !nhanVien.TrangThai)
                {
                    return false;
                }

                // So sánh mật khẩu plain text
                return nhanVien.MatKhau == password;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating password for employee {EmployeeId}", employeeId);
                return false;
            }
        }
    }
}
