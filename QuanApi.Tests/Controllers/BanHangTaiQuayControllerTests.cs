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
    public class BanHangTaiQuayControllerTests
    {
        private readonly DbContextOptions<BanQuanAu1DbContext> _options;
        private readonly Mock<ILogger<BanHangTaiQuayController>> _mockLogger;

        public BanHangTaiQuayControllerTests()
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
        public async Task TaoDonHang_WithValidData_ShouldCreateOrder()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            // Tạo dữ liệu test
            var nhanVienId = Guid.NewGuid();
            var khachHangId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();

            // Thêm nhân viên
            context.NhanViens.Add(new NhanVien
            {
                IDNhanVien = nhanVienId,
                TenNhanVien = "Test Employee",
                TrangThai = true
            });

            // Thêm sản phẩm chi tiết
            context.SanPhamChiTiets.Add(new SanPhamChiTiet
            {
                IDSanPhamChiTiet = sanPhamChiTietId,
                MaSPChiTiet = "SP001",
                SoLuong = 10,
                GiaBan = 100000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.TaoDonHangDto
            {
                IDNhanVien = nhanVienId,
                IDKhachHang = khachHangId,
                SanPhams = new List<BanHangTaiQuayController.SanPhamDto>
                {
                    new BanHangTaiQuayController.SanPhamDto
                    {
                        IDSanPhamChiTiet = sanPhamChiTietId,
                        SoLuong = 2
                    }
                }
            };

            // Act
            var result = await controller.TaoDonHang(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Anonymous>(okResult.Value);
            Assert.NotNull(response.hoaDonId);
            Assert.NotNull(response.maHoaDon);

            // Kiểm tra hóa đơn được tạo trong database
            var hoaDon = await context.HoaDons.FindAsync(response.hoaDonId);
            Assert.NotNull(hoaDon);
            Assert.Equal("ChuaThanhToan", hoaDon.TrangThai);
            Assert.Equal(200000, hoaDon.TongTien); // 2 * 100000
        }

        [Fact]
        public async Task TaoDonHang_WithInvalidProduct_ShouldReturnBadRequest()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var dto = new BanHangTaiQuayController.TaoDonHangDto
            {
                IDNhanVien = Guid.NewGuid(),
                IDKhachHang = Guid.NewGuid(),
                SanPhams = new List<BanHangTaiQuayController.SanPhamDto>
                {
                    new BanHangTaiQuayController.SanPhamDto
                    {
                        IDSanPhamChiTiet = Guid.NewGuid(), // Không tồn tại
                        SoLuong = 1
                    }
                }
            };

            // Act
            var result = await controller.TaoDonHang(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Anonymous>(badRequestResult.Value);
            Assert.Contains("Không tìm thấy sản phẩm chi tiết", response.message);
        }

        [Fact]
        public async Task TaoDonHang_WithInsufficientStock_ShouldReturnBadRequest()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var sanPhamChiTietId = Guid.NewGuid();
            context.SanPhamChiTiets.Add(new SanPhamChiTiet
            {
                IDSanPhamChiTiet = sanPhamChiTietId,
                MaSPChiTiet = "SP001",
                SoLuong = 5, // Chỉ có 5 sản phẩm
                GiaBan = 100000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.TaoDonHangDto
            {
                IDNhanVien = Guid.NewGuid(),
                IDKhachHang = Guid.NewGuid(),
                SanPhams = new List<BanHangTaiQuayController.SanPhamDto>
                {
                    new BanHangTaiQuayController.SanPhamDto
                    {
                        IDSanPhamChiTiet = sanPhamChiTietId,
                        SoLuong = 10 // Yêu cầu 10 sản phẩm
                    }
                }
            };

            // Act
            var result = await controller.TaoDonHang(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Anonymous>(badRequestResult.Value);
            Assert.Contains("Số lượng vượt quá tồn kho", response.message);
        }

        [Fact]
        public async Task ThemSanPham_WithValidData_ShouldAddProductToOrder()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var hoaDonId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();

            // Tạo hóa đơn
            context.HoaDons.Add(new HoaDon
            {
                IDHoaDon = hoaDonId,
                MaHoaDon = "HD001",
                TrangThai = "ChuaThanhToan",
                TrangThaiHoaDon = true
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

            var dto = new BanHangTaiQuayController.ThemSanPhamDto
            {
                IDHoaDon = hoaDonId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 3
            };

            // Act
            var result = await controller.ThemSanPham(dto);

            // Assert
            Assert.IsType<OkResult>(result);

            // Kiểm tra chi tiết hóa đơn được tạo
            var chiTietHoaDon = await context.ChiTietHoaDons
                .FirstOrDefaultAsync(x => x.IDHoaDon == hoaDonId && x.IDSanPhamChiTiet == sanPhamChiTietId);
            Assert.NotNull(chiTietHoaDon);
            Assert.Equal(3, chiTietHoaDon.SoLuong);
            Assert.Equal(300000, chiTietHoaDon.ThanhTien);

            // Kiểm tra tổng tiền hóa đơn được cập nhật
            var hoaDon = await context.HoaDons.FindAsync(hoaDonId);
            Assert.Equal(300000, hoaDon.TongTien);
        }

        [Fact]
        public async Task ThemSanPham_WithExistingProduct_ShouldUpdateQuantity()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var hoaDonId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();

            // Tạo hóa đơn
            context.HoaDons.Add(new HoaDon
            {
                IDHoaDon = hoaDonId,
                MaHoaDon = "HD001",
                TrangThai = "ChuaThanhToan",
                TrangThaiHoaDon = true
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

            // Tạo chi tiết hóa đơn hiện có
            context.ChiTietHoaDons.Add(new ChiTietHoaDon
            {
                IDChiTietHoaDon = Guid.NewGuid(),
                IDHoaDon = hoaDonId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 2,
                DonGia = 100000,
                ThanhTien = 200000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.ThemSanPhamDto
            {
                IDHoaDon = hoaDonId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 3
            };

            // Act
            var result = await controller.ThemSanPham(dto);

            // Assert
            Assert.IsType<OkResult>(result);

            // Kiểm tra số lượng được cập nhật
            var chiTietHoaDon = await context.ChiTietHoaDons
                .FirstOrDefaultAsync(x => x.IDHoaDon == hoaDonId && x.IDSanPhamChiTiet == sanPhamChiTietId);
            Assert.Equal(5, chiTietHoaDon.SoLuong); // 2 + 3
            Assert.Equal(500000, chiTietHoaDon.ThanhTien); // 5 * 100000
        }

        [Fact]
        public async Task CapNhatSanPham_WithZeroQuantity_ShouldRemoveProduct()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var hoaDonId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();

            // Tạo hóa đơn
            context.HoaDons.Add(new HoaDon
            {
                IDHoaDon = hoaDonId,
                MaHoaDon = "HD001",
                TrangThai = "ChuaThanhToan",
                TrangThaiHoaDon = true
            });

            // Tạo chi tiết hóa đơn
            var chiTietId = Guid.NewGuid();
            context.ChiTietHoaDons.Add(new ChiTietHoaDon
            {
                IDChiTietHoaDon = chiTietId,
                IDHoaDon = hoaDonId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuong = 2,
                DonGia = 100000,
                ThanhTien = 200000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.CapNhatSanPhamDto
            {
                IDHoaDon = hoaDonId,
                IDSanPhamChiTiet = sanPhamChiTietId,
                SoLuongMoi = 0
            };

            // Act
            var result = await controller.CapNhatSanPham(dto);

            // Assert
            Assert.IsType<OkResult>(result);

            // Kiểm tra chi tiết hóa đơn đã bị xóa
            var chiTietHoaDon = await context.ChiTietHoaDons.FindAsync(chiTietId);
            Assert.Null(chiTietHoaDon);
        }

        [Fact]
        public async Task ChonKhachHang_WithValidPhone_ShouldReturnCustomer()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var khachHangId = Guid.NewGuid();
            context.KhachHang.Add(new KhachHang
            {
                IDKhachHang = khachHangId,
                TenKhachHang = "Test Customer",
                SoDienThoai = "0123456789",
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.ChonKhachHangDto
            {
                SoDienThoai = "0123456789"
            };

            // Act
            var result = await controller.ChonKhachHang(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Anonymous>(okResult.Value);
            Assert.Equal(khachHangId, response.id);
            Assert.Equal("Test Customer", response.tenKhachHang);
            Assert.Equal("0123456789", response.sdt);
        }

        [Fact]
        public async Task ChonKhachHang_WithInvalidPhone_ShouldReturnNotFound()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var dto = new BanHangTaiQuayController.ChonKhachHangDto
            {
                SoDienThoai = "9999999999"
            };

            // Act
            var result = await controller.ChonKhachHang(dto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task TaoKhachHang_WithValidData_ShouldCreateCustomer()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var dto = new BanHangTaiQuayController.TaoKhachHangDto
            {
                TenKhachHang = "New Customer",
                SoDienThoai = "0987654321"
            };

            // Act
            var result = await controller.TaoKhachHang(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Anonymous>(okResult.Value);
            Assert.NotNull(response.id);
            Assert.Equal("New Customer", response.tenKhachHang);
            Assert.Equal("0987654321", response.sdt);

            // Kiểm tra khách hàng được tạo trong database
            var khachHang = await context.KhachHang
                .FirstOrDefaultAsync(x => x.SoDienThoai == "0987654321");
            Assert.NotNull(khachHang);
            Assert.Equal("New Customer", khachHang.TenKhachHang);
        }

        [Fact]
        public async Task TaoKhachHang_WithDuplicatePhone_ShouldReturnBadRequest()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            // Tạo khách hàng hiện có
            context.KhachHang.Add(new KhachHang
            {
                IDKhachHang = Guid.NewGuid(),
                TenKhachHang = "Existing Customer",
                SoDienThoai = "0123456789",
                TrangThai = true
            });

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.TaoKhachHangDto
            {
                TenKhachHang = "New Customer",
                SoDienThoai = "0123456789" // Số điện thoại đã tồn tại
            };

            // Act
            var result = await controller.TaoKhachHang(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Số điện thoại đã tồn tại.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetProducts_ShouldReturnProductList()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var sanPhamId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();

            // Tạo sản phẩm
            context.SanPhams.Add(new SanPham
            {
                IDSanPham = sanPhamId,
                TenSanPham = "Test Product",
                TrangThai = true
            });

            // Tạo sản phẩm chi tiết
            context.SanPhamChiTiets.Add(new SanPhamChiTiet
            {
                IDSanPhamChiTiet = sanPhamChiTietId,
                IDSanPham = sanPhamId,
                MaSPChiTiet = "SP001",
                SoLuong = 10,
                GiaBan = 100000,
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

            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<object>>(okResult.Value);
            Assert.NotEmpty(products);
        }

        [Fact]
        public async Task PayInvoice_WithValidData_ShouldCreateInvoice()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

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

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.InvoiceDto
            {
                CustomerId = khachHangId,
                CustomerName = "Test Customer",
                CustomerPhone = "0123456789",
                Address = "Test Address",
                Products = new List<BanHangTaiQuayController.InvoiceProductDto>
                {
                    new BanHangTaiQuayController.InvoiceProductDto
                    {
                        ProductDetailId = sanPhamChiTietId,
                        Quantity = 2
                    }
                },
                PaymentMethod = "cash",
                Shipping = false
            };

            // Act
            var result = await controller.PayInvoice(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Anonymous>(okResult.Value);
            Assert.NotNull(response.hoaDonId);
            Assert.NotNull(response.maHoaDon);

            // Kiểm tra hóa đơn được tạo
            var hoaDon = await context.HoaDons.FindAsync(response.hoaDonId);
            Assert.NotNull(hoaDon);
            Assert.Equal("DaThanhToan", hoaDon.TrangThai);
            Assert.Equal(200000, hoaDon.TongTien);

            // Kiểm tra số lượng sản phẩm được trừ
            var sanPhamChiTiet = await context.SanPhamChiTiets.FindAsync(sanPhamChiTietId);
            Assert.Equal(8, sanPhamChiTiet.SoLuong); // 10 - 2
        }

        [Fact]
        public async Task PayInvoice_WithInvalidPaymentMethod_ShouldReturnBadRequest()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var dto = new BanHangTaiQuayController.InvoiceDto
            {
                CustomerId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                CustomerPhone = "0123456789",
                Products = new List<BanHangTaiQuayController.InvoiceProductDto>(),
                PaymentMethod = "invalid_method",
                Shipping = false
            };

            // Act
            var result = await controller.PayInvoice(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Phương thức thanh toán không hợp lệ.", badRequestResult.Value);
        }

        [Fact]
        public async Task PayInvoice_WithInsufficientStock_ShouldReturnBadRequest()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

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

            // Tạo sản phẩm chi tiết với số lượng ít
            context.SanPhamChiTiets.Add(new SanPhamChiTiet
            {
                IDSanPhamChiTiet = sanPhamChiTietId,
                MaSPChiTiet = "SP001",
                SoLuong = 1, // Chỉ có 1 sản phẩm
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

            await context.SaveChangesAsync();

            var dto = new BanHangTaiQuayController.InvoiceDto
            {
                CustomerId = khachHangId,
                CustomerName = "Test Customer",
                CustomerPhone = "0123456789",
                Products = new List<BanHangTaiQuayController.InvoiceProductDto>
                {
                    new BanHangTaiQuayController.InvoiceProductDto
                    {
                        ProductDetailId = sanPhamChiTietId,
                        Quantity = 5 // Yêu cầu 5 sản phẩm
                    }
                },
                PaymentMethod = "cash",
                Shipping = false
            };

            // Act
            var result = await controller.PayInvoice(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<Anonymous>(badRequestResult.Value);
            Assert.Contains("vượt quá tồn kho", response.message);
        }

        [Fact]
        public async Task GetPaymentMethods_ShouldReturnPaymentMethods()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            // Tạo phương thức thanh toán
            context.PhuongThucThanhToans.AddRange(
                new PhuongThucThanhToan
                {
                    IDPhuongThucThanhToan = Guid.NewGuid(),
                    MaPhuongThuc = "cash",
                    TenPhuongThuc = "Tiền mặt",
                    TrangThai = true
                },
                new PhuongThucThanhToan
                {
                    IDPhuongThucThanhToan = Guid.NewGuid(),
                    MaPhuongThuc = "bank",
                    TenPhuongThuc = "Chuyển khoản",
                    TrangThai = true
                }
            );

            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetPaymentMethods();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var methods = Assert.IsType<List<object>>(okResult.Value);
            Assert.Equal(2, methods.Count);
        }

        [Fact]
        public async Task GetCustomerDiscountVouchers_WithValidCustomer_ShouldReturnVouchers()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var khachHangId = Guid.NewGuid();
            var phieuGiamGiaId = Guid.NewGuid();

            // Tạo khách hàng
            context.KhachHang.Add(new KhachHang
            {
                IDKhachHang = khachHangId,
                TenKhachHang = "Test Customer",
                TrangThai = true
            });

            // Tạo phiếu giảm giá
            context.PhieuGiamGias.Add(new PhieuGiamGia
            {
                IDPhieuGiamGia = phieuGiamGiaId,
                MaCode = "DISCOUNT10",
                TenPhieu = "Giảm 10%",
                GiaTriGiam = 10,
                NgayBatDau = DateTime.Now.AddDays(-1),
                NgayKetThuc = DateTime.Now.AddDays(1),
                TrangThai = true
            });

            // Tạo khách hàng phiếu giảm giá
            context.KhachHangPhieuGiams.Add(new KhachHangPhieuGiam
            {
                IDKhachHang = khachHangId,
                IDPhieuGiamGia = phieuGiamGiaId,
                SoLuong = 5,
                SoLuongDaSuDung = 2,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetCustomerDiscountVouchers(khachHangId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var vouchers = Assert.IsType<List<object>>(okResult.Value);
            Assert.NotEmpty(vouchers);
        }

        [Fact]
        public async Task GetProductDetail_WithValidId_ShouldReturnProduct()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            var sanPhamId = Guid.NewGuid();
            var sanPhamChiTietId = Guid.NewGuid();
            var kichCoId = Guid.NewGuid();
            var mauSacId = Guid.NewGuid();

            // Tạo sản phẩm
            context.SanPhams.Add(new SanPham
            {
                IDSanPham = sanPhamId,
                TenSanPham = "Test Product",
                TrangThai = true
            });

            // Tạo kích cỡ
            context.KichCos.Add(new KichCo
            {
                IDKichCo = kichCoId,
                TenKichCo = "M",
                TrangThai = true
            });

            // Tạo màu sắc
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
                SoLuong = 10,
                GiaBan = 100000,
                TrangThai = true
            });

            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetProductDetail(sanPhamChiTietId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var product = Assert.IsType<Anonymous>(okResult.Value);
            Assert.Equal(sanPhamChiTietId, product.id);
            Assert.Equal("Test Product", product.name);
            Assert.Equal(100000, product.price);
        }

        [Fact]
        public async Task GetProductDetail_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            // Act
            var result = await controller.GetProductDetail(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        // Helper class for anonymous objects
        private class Anonymous
        {
            public Guid? hoaDonId { get; set; }
            public string maHoaDon { get; set; }
            public string message { get; set; }
            public Guid? id { get; set; }
            public string tenKhachHang { get; set; }
            public string sdt { get; set; }
        }
    }
}
