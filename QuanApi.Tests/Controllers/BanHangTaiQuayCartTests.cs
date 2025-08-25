using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QuanApi.Controllers;
using QuanApi.Data;
using BanQuanAu1.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QuanApi.Tests.Controllers
{
    public class BanHangTaiQuayCartTests
    {
        private readonly DbContextOptions<BanQuanAu1DbContext> _options;
        private readonly Mock<ILogger<BanHangTaiQuayController>> _mockLogger;

        public BanHangTaiQuayCartTests()
        {
            _options = new DbContextOptionsBuilder<BanQuanAu1DbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _mockLogger = new Mock<ILogger<BanHangTaiQuayController>>();
        }

        private BanQuanAu1DbContext CreateContext()
        {
            return new BanQuanAu1DbContext(_options);
        }

        private BanHangTaiQuayController CreateController(BanQuanAu1DbContext context)
        {
            return new BanHangTaiQuayController(context);
        }

        [Fact]
        public async Task TaoGioHang_WithValidData_ShouldCreateCart()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var khachHangId = Guid.NewGuid();
            var dto = new BanHangTaiQuayController.TaoGioHangDto
            {
                IDKhachHang = khachHangId,
                NguoiTao = "Test User"
            };

            // Act
            var result = await controller.TaoGioHang(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Anonymous>(okResult.Value);
            Assert.NotNull(response.idGioHang);
            Assert.NotNull(response.maGioHang);

            // Kiểm tra giỏ hàng được tạo trong database
            var gioHang = await context.GioHangs.FindAsync(response.idGioHang);
            Assert.NotNull(gioHang);
            Assert.Equal(khachHangId, gioHang.IDKhachHang);
            Assert.True(gioHang.TrangThai);
        }

        [Fact]
        public async Task ThemVaoGioHang_WithValidData_ShouldAddProductToCart()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var gioHangId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();

            // Tạo giỏ hàng
            context.GioHangs.Add(new GioHang
            {
                IDGioHang = gioHangId,
                MaGioHang = "GH001",
                TrangThai = true
            });

            // Tạo sản phẩm chi tiết
            context.SanPhamChiTiets.Add(new SanPhamChiTiet
            {
                IDSanPhamChiTiet = sanPhamChiTietId,
                MaSPChiTiet = "SP001",
                SoLuong = 10,
                GiaBan = 100000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.ThemVaoGioHangDto
            {
                IDGioHang = gioHangId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 3,
                NguoiCapNhat = "Test User"
            };

            // Act
            var result = await controller.ThemVaoGioHang(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Anonymous>(okResult.Value);
            Assert.Equal("Thêm vào giỏ hàng thành công", response.message);
            Assert.Equal(7, response.soLuongConLai); // 10 - 3

            // Kiểm tra chi tiết giỏ hàng được tạo
            var chiTietGioHang = await context.ChiTietGioHangs
                .FirstOrDefaultAsync(x => x.IDGioHang == gioHangId && x.IDSanPhamChiTiet == sanPhamChiTietId);
            Assert.NotNull(chiTietGioHang);
            Assert.Equal(3, chiTietGioHang.SoLuong);
            Assert.Equal(100000, chiTietGioHang.GiaBan);
        }

        [Fact]
        public async Task ThemVaoGioHang_WithExistingProduct_ShouldUpdateQuantity()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var gioHangId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();

            // Tạo giỏ hàng
            context.GioHangs.Add(new GioHang
            {
                IDGioHang = gioHangId,
                MaGioHang = "GH001",
                TrangThai = true
            });

            // Tạo sản phẩm chi tiết
            context.SanPhamChiTiets.Add(new SanPhamChiTiet
            {
                IDSanPhamChiTiet = sanPhamChiTietId,
                MaSPChiTiet = "SP001",
                SoLuong = 10,
                GiaBan = 100000,
                TrangThai = true
            });

            // Tạo chi tiết giỏ hàng hiện có
            context.ChiTietGioHangs.Add(new ChiTietGioHang
            {
                IDChiTietGioHang = Guid.NewGuid(),
                IDGioHang = gioHangId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 2,
                GiaBan = 100000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.ThemVaoGioHangDto
            {
                IDGioHang = gioHangId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 3,
                NguoiCapNhat = "Test User"
            };

            // Act
            var result = await controller.ThemVaoGioHang(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Anonymous>(okResult.Value);
            Assert.Equal("Thêm vào giỏ hàng thành công", response.message);

            // Kiểm tra số lượng được cập nhật
            var chiTietGioHang = await context.ChiTietGioHangs
                .FirstOrDefaultAsync(x => x.IDGioHang == gioHangId && x.IDSanPhamChiTiet == sanPhamChiTietId);
            Assert.Equal(5, chiTietGioHang.SoLuong); // 2 + 3
        }

        [Fact]
        public async Task ThemVaoGioHang_WithInsufficientStock_ShouldReturnBadRequest()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var gioHangId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();

            // Tạo giỏ hàng
            context.GioHangs.Add(new GioHang
            {
                IDGioHang = gioHangId,
                MaGioHang = "GH001",
                TrangThai = true
            });

            // Tạo sản phẩm chi tiết với số lượng ít
            context.SanPhamChiTiets.Add(new SanPhamChiTiet
            {
                IDSanPhamChiTiet = sanPhamChiTietId,
                MaSPChiTiet = "SP001",
                SoLuong = 2, // Chỉ có 2 sản phẩm
                GiaBan = 100000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.ThemVaoGioHangDto
            {
                IDGioHang = gioHangId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 5, // Yêu cầu 5 sản phẩm
                NguoiCapNhat = "Test User"
            };

            // Act
            var result = await controller.ThemVaoGioHang(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Anonymous>(badRequestResult.Value);
            Assert.Contains("Số lượng vượt quá tồn kho", response.message);
        }

        [Fact]
        public async Task CapNhatSoLuongGioHang_WithValidData_ShouldUpdateQuantity()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var gioHangId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();

            // Tạo giỏ hàng
            context.GioHangs.Add(new GioHang
            {
                IDGioHang = gioHangId,
                MaGioHang = "GH001",
                TrangThai = true
            });

            // Tạo sản phẩm chi tiết
            context.SanPhamChiTiets.Add(new SanPhamChiTiet
            {
                IDSanPhamChiTiet = sanPhamChiTietId,
                MaSPChiTiet = "SP001",
                SoLuong = 8, // Đã trừ 2 sản phẩm trước đó
                GiaBan = 100000,
                TrangThai = true
            });

            // Tạo chi tiết giỏ hàng
            context.ChiTietGioHangs.Add(new ChiTietGioHang
            {
                IDChiTietGioHang = Guid.NewGuid(),
                IDGioHang = gioHangId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 2,
                GiaBan = 100000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.CapNhatSoLuongGioHangDto
            {
                IDGioHang = gioHangId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuongMoi = 4,
                NguoiCapNhat = "Test User"
            };

            // Act
            var result = await controller.CapNhatSoLuongGioHang(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Anonymous>(okResult.Value);
            Assert.Equal("Cập nhật số lượng thành công", response.message);

            // Kiểm tra số lượng được cập nhật
            var chiTietGioHang = await context.ChiTietGioHangs
                .FirstOrDefaultAsync(x => x.IDGioHang == gioHangId && x.IDSanPhamChiTiet == sanPhamChiTietId);
            Assert.Equal(4, chiTietGioHang.SoLuong);

            // Kiểm tra tồn kho được cập nhật
            var sanPhamChiTiet = await context.SanPhamChiTiets.FindAsync(sanPhamChiTietId);
            Assert.Equal(6, sanPhamChiTiet.SoLuong); // 8 - 2 (số lượng cũ) + 2 (số lượng mới) = 8, nhưng đã trừ 2 nên còn 6
        }

        [Fact]
        public async Task CapNhatSoLuongGioHang_WithZeroQuantity_ShouldRemoveProduct()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var gioHangId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();
            var chiTietId = Guid.NewGuid();

            // Tạo giỏ hàng
            context.GioHangs.Add(new GioHang
            {
                IDGioHang = gioHangId,
                MaGioHang = "GH001",
                TrangThai = true
            });

            // Tạo sản phẩm chi tiết
            context.SanPhamChiTiets.Add(new SanPhamChiTiet
            {
                IDSanPhamChiTiet = sanPhamChiTietId,
                MaSPChiTiet = "SP001",
                SoLuong = 8,
                GiaBan = 100000,
                TrangThai = true
            });

            // Tạo chi tiết giỏ hàng
            context.ChiTietGioHangs.Add(new ChiTietGioHang
            {
                IDChiTietGioHang = chiTietId,
                IDGioHang = gioHangId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 2,
                GiaBan = 100000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.CapNhatSoLuongGioHangDto
            {
                IDGioHang = gioHangId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuongMoi = 0,
                NguoiCapNhat = "Test User"
            };

            // Act
            var result = await controller.CapNhatSoLuongGioHang(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Anonymous>(okResult.Value);
            Assert.Equal("Cập nhật số lượng thành công", response.message);

            // Kiểm tra chi tiết giỏ hàng đã bị xóa
            var chiTietGioHang = await context.ChiTietGioHangs.FindAsync(chiTietId);
            Assert.Null(chiTietGioHang);

            // Kiểm tra tồn kho được hoàn trả
            var sanPhamChiTiet = await context.SanPhamChiTiets.FindAsync(sanPhamChiTietId);
            Assert.Equal(10, sanPhamChiTiet.SoLuong); // 8 + 2
        }

        [Fact]
        public async Task XoaKhoiGioHang_WithValidData_ShouldRemoveProduct()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var gioHangId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();
            var chiTietId = Guid.NewGuid();

            // Tạo giỏ hàng
            context.GioHangs.Add(new GioHang
            {
                IDGioHang = gioHangId,
                MaGioHang = "GH001",
                TrangThai = true
            });

            // Tạo sản phẩm chi tiết
            context.SanPhamChiTiets.Add(new SanPhamChiTiet
            {
                IDSanPhamChiTiet = sanPhamChiTietId,
                MaSPChiTiet = "SP001",
                SoLuong = 8,
                GiaBan = 100000,
                TrangThai = true
            });

            // Tạo chi tiết giỏ hàng
            context.ChiTietGioHangs.Add(new ChiTietGioHang
            {
                IDChiTietGioHang = chiTietId,
                IDGioHang = gioHangId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 2,
                GiaBan = 100000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.XoaKhoiGioHangDto
            {
                IDGioHang = gioHangId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                NguoiCapNhat = "Test User"
            };

            // Act
            var result = await controller.XoaKhoiGioHang(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Anonymous>(okResult.Value);
            Assert.Equal("Xóa sản phẩm khỏi giỏ hàng thành công", response.message);

            // Kiểm tra chi tiết giỏ hàng đã bị xóa
            var chiTietGioHang = await context.ChiTietGioHangs.FindAsync(chiTietId);
            Assert.Null(chiTietGioHang);

            // Kiểm tra tồn kho được hoàn trả
            var sanPhamChiTiet = await context.SanPhamChiTiets.FindAsync(sanPhamChiTietId);
            Assert.Equal(10, sanPhamChiTiet.SoLuong); // 8 + 2
        }

        [Fact]
        public async Task GetGioHang_WithValidId_ShouldReturnCart()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var gioHangId = Guid.NewGuid();
            var khachHangId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();
            var sanPhamId = Guid.NewGuid();

            // Tạo khách hàng
            context.KhachHang.Add(new KhachHang
            {
                IDKhachHang = khachHangId,
                TenKhachHang = "Test Customer",
                SoDienThoai = "0123456789",
                Email = "test@example.com",
                TrangThai = true
            });

            // Tạo sản phẩm
            context.SanPhams.Add(new SanPham
            {
                IDSanPham = sanPhamId,
                TenSanPham = "Test Product",
                TrangThai = true
            });

            // Tạo kích cỡ
            var kichCoId = Guid.NewGuid();
            context.KichCos.Add(new KichCo
            {
                IDKichCo = kichCoId,
                TenKichCo = "M",
                TrangThai = true
            });

            // Tạo màu sắc
            var mauSacId = Guid.NewGuid();
            context.MauSacs.Add(new MauSac
            {
                IDMauSac = mauSacId,
                TenMauSac = "Đỏ",
                TrangThai = true
            });

            // Tạo sản phẩm chi tiết
            context.SanPhamChiTiets.Add(new SanPhamChiTiet
            {
                IDSanPhamChiTiet = sanPhamChiTietId,
                IDSanPham = sanPhamId,
                IDKichCo = kichCoId,
                IDMauSac = mauSacId,
                MaSPChiTiet = "SP001",
                SoLuong = 8,
                GiaBan = 100000,
                TrangThai = true
            });

            // Tạo giỏ hàng
            context.GioHangs.Add(new GioHang
            {
                IDGioHang = gioHangId,
                IDKhachHang = khachHangId,
                MaGioHang = "GH001",
                TrangThai = true
            });

            // Tạo chi tiết giỏ hàng
            context.ChiTietGioHangs.Add(new ChiTietGioHang
            {
                IDChiTietGioHang = Guid.NewGuid(),
                IDGioHang = gioHangId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 2,
                GiaBan = 100000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetGioHang(gioHangId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Anonymous>(okResult.Value);
            Assert.Equal(gioHangId, response.idGioHang);
            Assert.Equal("GH001", response.maGioHang);
            Assert.NotNull(response.khachHang);
            Assert.Equal(khachHangId, response.khachHang.id);
            Assert.Equal("Test Customer", response.khachHang.ten);
            Assert.Equal(200000, response.tongTien); // 2 * 100000
            Assert.NotEmpty(response.sanPhams);
        }

        [Fact]
        public async Task GetGioHang_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            // Act
            var result = await controller.GetGioHang(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task XoaGioHang_WithValidData_ShouldDeleteCart()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var gioHangId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();

            // Tạo giỏ hàng
            context.GioHangs.Add(new GioHang
            {
                IDGioHang = gioHangId,
                MaGioHang = "GH001",
                TrangThai = true
            });

            // Tạo sản phẩm chi tiết
            context.SanPhamChiTiets.Add(new SanPhamChiTiet
            {
                IDSanPhamChiTiet = sanPhamChiTietId,
                MaSPChiTiet = "SP001",
                SoLuong = 8,
                GiaBan = 100000,
                TrangThai = true
            });

            // Tạo chi tiết giỏ hàng
            context.ChiTietGioHangs.Add(new ChiTietGioHang
            {
                IDChiTietGioHang = Guid.NewGuid(),
                IDGioHang = gioHangId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 2,
                GiaBan = 100000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.XoaGioHangDto
            {
                IDGioHang = gioHangId,
                NguoiCapNhat = "Test User"
            };

            // Act
            var result = await controller.XoaGioHang(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Anonymous>(okResult.Value);
            Assert.Equal("Xóa giỏ hàng thành công", response.message);

            // Kiểm tra giỏ hàng đã bị xóa
            var gioHang = await context.GioHangs.FindAsync(gioHangId);
            Assert.Null(gioHang);

            // Kiểm tra tồn kho được hoàn trả
            var sanPhamChiTiet = await context.SanPhamChiTiets.FindAsync(sanPhamChiTietId);
            Assert.Equal(10, sanPhamChiTiet.SoLuong); // 8 + 2
        }

        [Fact]
        public async Task ChuyenGioHangThanhHoaDon_WithValidData_ShouldConvertCartToInvoice()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var gioHangId = Guid.NewGuid();
            var khachHangId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();
            var phuongThucThanhToanId = Guid.NewGuid();

            // Tạo khách hàng
            context.KhachHang.Add(new KhachHang
            {
                IDKhachHang = khachHangId,
                TenKhachHang = "Test Customer",
                SoDienThoai = "0123456789",
                TrangThai = true
            });

            // Tạo sản phẩm chi tiết
            context.SanPhamChiTiets.Add(new SanPhamChiTiet
            {
                IDSanPhamChiTiet = sanPhamChiTietId,
                MaSPChiTiet = "SP001",
                SoLuong = 10,
                GiaBan = 100000,
                TrangThai = true
            });

            // Tạo phương thức thanh toán
            context.PhuongThucThanhToans.Add(new PhuongThucThanhToan
            {
                IDPhuongThucThanhToan = phuongThucThanhToanId,
                MaPhuongThuc = "cash",
                TenPhuongThuc = "Tiền mặt",
                TrangThai = true
            });

            // Tạo giỏ hàng
            context.GioHangs.Add(new GioHang
            {
                IDGioHang = gioHangId,
                IDKhachHang = khachHangId,
                MaGioHang = "GH001",
                TrangThai = true
            });

            // Tạo chi tiết giỏ hàng
            context.ChiTietGioHangs.Add(new ChiTietGioHang
            {
                IDChiTietGioHang = Guid.NewGuid(),
                IDGioHang = gioHangId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 2,
                GiaBan = 100000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.ChuyenGioHangThanhHoaDonDto
            {
                IDGioHang = gioHangId,
                CustomerName = "Test Customer",
                CustomerPhone = "0123456789",
                Address = "Test Address",
                PaymentMethod = "cash",
                Shipping = false
            };

            // Act
            var result = await controller.ChuyenGioHangThanhHoaDon(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Anonymous>(okResult.Value);
            Assert.NotNull(response.hoaDonId);
            Assert.NotNull(response.maHoaDon);
            Assert.Equal("Chuyển giỏ hàng thành hóa đơn thành công", response.message);

            // Kiểm tra hóa đơn được tạo
            var hoaDon = await context.HoaDons.FindAsync(response.hoaDonId);
            Assert.NotNull(hoaDon);
            Assert.Equal("DaThanhToan", hoaDon.TrangThai);
            Assert.Equal(200000, hoaDon.TongTien);

            // Kiểm tra giỏ hàng đã bị xóa
            var gioHang = await context.GioHangs.FindAsync(gioHangId);
            Assert.Null(gioHang);
        }

        [Fact]
        public async Task ChuyenGioHangThanhHoaDon_WithEmptyCart_ShouldReturnBadRequest()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var gioHangId = Guid.NewGuid();
            var khachHangId = Guid.NewGuid();

            // Tạo khách hàng
            context.KhachHang.Add(new KhachHang
            {
                IDKhachHang = khachHangId,
                TenKhachHang = "Test Customer",
                SoDienThoai = "0123456789",
                TrangThai = true
            });

            // Tạo giỏ hàng rỗng
            context.GioHangs.Add(new GioHang
            {
                IDGioHang = gioHangId,
                IDKhachHang = khachHangId,
                MaGioHang = "GH001",
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.ChuyenGioHangThanhHoaDonDto
            {
                IDGioHang = gioHangId,
                CustomerName = "Test Customer",
                CustomerPhone = "0123456789",
                PaymentMethod = "cash",
                Shipping = false
            };

            // Act
            var result = await controller.ChuyenGioHangThanhHoaDon(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Anonymous>(badRequestResult.Value);
            Assert.Equal("Giỏ hàng không có sản phẩm nào.", response.message);
        }

        // Helper class for anonymous objects
        private class Anonymous
        {
            public Guid? idGioHang { get; set; }
            public string maGioHang { get; set; }
            public string message { get; set; }
            public int soLuongConLai { get; set; }
            public Guid? hoaDonId { get; set; }
            public string maHoaDon { get; set; }
            public decimal tongTien { get; set; }
            public CustomerInfo khachHang { get; set; }
            public List<object> sanPhams { get; set; }
        }

        private class CustomerInfo
        {
            public Guid? id { get; set; }
            public string ten { get; set; }
            public string sdt { get; set; }
            public string email { get; set; }
        }
    }
}
