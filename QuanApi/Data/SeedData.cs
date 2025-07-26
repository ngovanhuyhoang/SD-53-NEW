using Microsoft.EntityFrameworkCore;
using QuanApi.Data;

namespace QuanApi.Data
{
    public static class SeedData
    {
        public static void SeedPhuongThucThanhToan(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PhuongThucThanhToan>().HasData(
                new PhuongThucThanhToan
                {
                    IDPhuongThucThanhToan = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    MaPhuongThuc = "TIEN_MAT",
                    TenPhuongThuc = "Thanh toán tiền mặt",
                    MoTa = "Thanh toán bằng tiền mặt khi nhận hàng",
                    NgayTao = DateTime.UtcNow,
                    NguoiTao = "System",
                    TrangThai = true
                },
                new PhuongThucThanhToan
                {
                    IDPhuongThucThanhToan = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    MaPhuongThuc = "CHUYEN_KHOAN",
                    TenPhuongThuc = "Chuyển khoản ngân hàng",
                    MoTa = "Thanh toán bằng chuyển khoản ngân hàng",
                    NgayTao = DateTime.UtcNow,
                    NguoiTao = "System",
                    TrangThai = true
                },
                new PhuongThucThanhToan
                {
                    IDPhuongThucThanhToan = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    MaPhuongThuc = "VIETTEL_PAY",
                    TenPhuongThuc = "ViettelPay",
                    MoTa = "Thanh toán qua ví điện tử ViettelPay",
                    NgayTao = DateTime.UtcNow,
                    NguoiTao = "System",
                    TrangThai = true
                },
                new PhuongThucThanhToan
                {
                    IDPhuongThucThanhToan = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    MaPhuongThuc = "MOMO",
                    TenPhuongThuc = "MoMo",
                    MoTa = "Thanh toán qua ví điện tử MoMo",
                    NgayTao = DateTime.UtcNow,
                    NguoiTao = "System",
                    TrangThai = true
                }
            );
        }
    }
} 