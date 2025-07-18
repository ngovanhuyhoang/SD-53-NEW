using BanQuanAu1.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuanView.Controllers
{
    public class LoginController : Controller
    {
        private readonly BanQuanAu1DbContext _context;

        public LoginController(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Error = TempData["Error"];
            return View();
        }

        public IActionResult Login(string? returnUrl = "/")
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", new { returnUrl })
            };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse(string? returnUrl = "/")
        {
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var avatar = User.FindFirst("picture")?.Value;

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded || result.Principal == null)
            {
                TempData["Error"] = "Đăng nhập thất bại hoặc bị hủy. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }

             email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Không thể lấy email từ tài khoản Google.";
                return RedirectToAction("Index");
            }

            // Chuẩn hóa email
            email = email.Trim().ToLower();
            Console.WriteLine($"Email Google: {email}");

            // Tìm nhân viên trong cơ sở dữ liệu
            var nhanVien = await _context.NhanViens
                .Include(nv => nv.VaiTro)
                .FirstOrDefaultAsync(nv => nv.Email != null && nv.Email.Trim().ToLower() == email && nv.TrangThai);

            if (nhanVien != null)
            {
                Console.WriteLine($"Tìm thấy NhanVien: {nhanVien.Email}, IDVaiTro: {nhanVien.IDVaiTro}, VaiTro: {(nhanVien.VaiTro != null ? nhanVien.VaiTro.MaVaiTro : "null")}");

                if (nhanVien.VaiTro != null && nhanVien.VaiTro.MaVaiTro?.ToLower() == "admin")
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, nhanVien.TenNhanVien ?? email.Split('@')[0]),
                        new Claim(ClaimTypes.Email, nhanVien.Email),
                        new Claim(ClaimTypes.Role, nhanVien.VaiTro.MaVaiTro),
                        new Claim("custom:id_nhanvien", nhanVien.IDNhanVien.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                    return RedirectToAction("Index", "ProductManage", new { area = "Admin" });

                }
                else
                {
                    Console.WriteLine($"NhanVien không có vai trò admin. VaiTro: {(nhanVien.VaiTro != null ? nhanVien.VaiTro.MaVaiTro : "null")}");
                }
            }
            else
            {
                Console.WriteLine($"Không tìm thấy NhanVien cho email: {email}");
            }

            // Tìm hoặc tạo khách hàng
            var khachHang = await _context.KhachHang
                .FirstOrDefaultAsync(kh => kh.Email != null && kh.Email.Trim().ToLower() == email && kh.TrangThai);

            if (khachHang == null)
            {
                khachHang = new KhachHang
                {
                    MaKhachHang = $"KH{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                    Email = email,
                    TenKhachHang = result.Principal.FindFirst(ClaimTypes.Name)?.Value ?? email.Split('@')[0],
                    SoDienThoai = "0000000000",
                    NgayTao = DateTime.Now,
                    TrangThai = true
                };
                _context.KhachHang.Add(khachHang);
                await _context.SaveChangesAsync();
            }

            var khachClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, khachHang.TenKhachHang),
                new Claim(ClaimTypes.Email, khachHang.Email),
                new Claim(ClaimTypes.Role, "KhachHang"),
                new Claim("custom:id_khachhang", khachHang.IDKhachHang.ToString())
            };

            var khachIdentity = new ClaimsIdentity(khachClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(khachIdentity));

            return LocalRedirect(returnUrl ?? "/");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}