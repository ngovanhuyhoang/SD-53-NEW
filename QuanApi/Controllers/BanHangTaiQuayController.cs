using Microsoft.AspNetCore.Mvc;
using QuanApi.Data;
using QuanApi.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BanQuanAu1.Web.Data;
using System.Collections.Generic; // Added for List

namespace QuanApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BanHangTaiQuayController : ControllerBase
    {
        private readonly BanQuanAu1DbContext _context;
        public BanHangTaiQuayController(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        // 1. Tạo đơn hàng mới
        [HttpPost("tao-don")]
        public async Task<IActionResult> TaoDonHang([FromBody] TaoDonHangDto dto)
        {
            // Kiểm tra số lượng đơn chưa thanh toán của nhân viên
            var soDonChuaThanhToan = await _context.HoaDons.CountAsync(h => h.IDNhanVien == dto.IDNhanVien && h.TrangThai == "ChuaThanhToan");
            if (soDonChuaThanhToan >= 5)
            {
                return BadRequest("Bạn chỉ được tạo tối đa 5 đơn hàng chưa thanh toán cùng lúc.");
            }

            var hoaDon = new HoaDon
            {
                IDHoaDon = Guid.NewGuid(),
                MaHoaDon = $"HD{DateTime.Now:yyyyMMddHHmmssfff}",
                IDNhanVien = dto.IDNhanVien,
                IDKhachHang = dto.IDKhachHang,
                IDPhuongThucThanhToan = Guid.Empty, // Chưa chọn
                TongTien = 0,
                TrangThai = "ChuaThanhToan",
                NgayTao = DateTime.Now,
                TrangThaiHoaDon = true
            };
            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();

            // Thêm sản phẩm nếu có
            if (dto.SanPhams != null && dto.SanPhams.Count > 0)
            {
                foreach (var sp in dto.SanPhams)
                {
                    var spct = await _context.SanPhamChiTiets.FindAsync(sp.IDSanPhamChiTiet);
                    if (spct == null)
                        return BadRequest($"Không tìm thấy sản phẩm chi tiết: {sp.IDSanPhamChiTiet}");
                    if (sp.SoLuong <= 0 || sp.SoLuong > spct.SoLuong)
                        return BadRequest($"Số lượng không hợp lệ cho sản phẩm: {spct.MaSPChiTiet}");
                    var cthd = new ChiTietHoaDon
                    {
                        IDChiTietHoaDon = Guid.NewGuid(),
                        MaChiTietHoaDon = $"CTHD{DateTime.Now:yyyyMMddHHmmssfff}",
                        IDHoaDon = hoaDon.IDHoaDon,
                        IDSanPhamChiTiet = sp.IDSanPhamChiTiet,
                        SoLuong = sp.SoLuong,
                        DonGia = spct.GiaBan,
                        ThanhTien = spct.GiaBan * sp.SoLuong,
                        NgayTao = DateTime.Now,
                        TrangThai = true
                    };
                    _context.ChiTietHoaDons.Add(cthd);
                    hoaDon.TongTien += cthd.ThanhTien;
                }
                await _context.SaveChangesAsync();
            }
            return Ok(new { hoaDon.IDHoaDon, hoaDon.MaHoaDon });
        }

        // 2. Thêm sản phẩm vào đơn
        [HttpPost("them-san-pham")]
        public async Task<IActionResult> ThemSanPham([FromBody] ThemSanPhamDto dto)
        {
            var hoaDon = await _context.HoaDons.FindAsync(dto.IDHoaDon);
            if (hoaDon == null || hoaDon.TrangThai != "ChuaThanhToan")
                return BadRequest("Đơn hàng không tồn tại hoặc đã thanh toán.");
            var spct = await _context.SanPhamChiTiets.FindAsync(dto.IDSanPhamChiTiet);
            if (spct == null)
                return BadRequest("Không tìm thấy sản phẩm chi tiết.");
            if (dto.SoLuong <= 0 || dto.SoLuong > spct.SoLuong)
                return BadRequest("Số lượng không hợp lệ.");
            var cthd = await _context.ChiTietHoaDons.FirstOrDefaultAsync(x => x.IDHoaDon == dto.IDHoaDon && x.IDSanPhamChiTiet == dto.IDSanPhamChiTiet);
            if (cthd != null)
            {
                cthd.SoLuong += dto.SoLuong;
                cthd.ThanhTien = cthd.SoLuong * spct.GiaBan;
            }
            else
            {
                cthd = new ChiTietHoaDon
                {
                    IDChiTietHoaDon = Guid.NewGuid(),
                    MaChiTietHoaDon = $"CTHD{DateTime.Now:yyyyMMddHHmmssfff}",
                    IDHoaDon = dto.IDHoaDon,
                    IDSanPhamChiTiet = dto.IDSanPhamChiTiet,
                    SoLuong = dto.SoLuong,
                    DonGia = spct.GiaBan,
                    ThanhTien = spct.GiaBan * dto.SoLuong,
                    NgayTao = DateTime.Now,
                    TrangThai = true
                };
                _context.ChiTietHoaDons.Add(cthd);
            }
            // Cập nhật tổng tiền hóa đơn
            hoaDon.TongTien = await _context.ChiTietHoaDons.Where(x => x.IDHoaDon == dto.IDHoaDon).SumAsync(x => x.ThanhTien);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // 3. Cập nhật/xóa sản phẩm trong đơn
        [HttpPost("cap-nhat-san-pham")]
        public async Task<IActionResult> CapNhatSanPham([FromBody] CapNhatSanPhamDto dto)
        {
            var hoaDon = await _context.HoaDons.FindAsync(dto.IDHoaDon);
            if (hoaDon == null || hoaDon.TrangThai != "ChuaThanhToan")
                return BadRequest("Đơn hàng không tồn tại hoặc đã thanh toán.");
            var cthd = await _context.ChiTietHoaDons.FirstOrDefaultAsync(x => x.IDHoaDon == dto.IDHoaDon && x.IDSanPhamChiTiet == dto.IDSanPhamChiTiet);
            if (cthd == null)
                return BadRequest("Sản phẩm không tồn tại trong đơn hàng.");
            if (dto.SoLuongMoi <= 0)
            {
                _context.ChiTietHoaDons.Remove(cthd);
            }
            else
            {
                var spct = await _context.SanPhamChiTiets.FindAsync(dto.IDSanPhamChiTiet);
                if (spct == null)
                    return BadRequest("Không tìm thấy sản phẩm chi tiết.");
                if (dto.SoLuongMoi > spct.SoLuong)
                    return BadRequest("Số lượng vượt quá tồn kho.");
                cthd.SoLuong = dto.SoLuongMoi;
                cthd.ThanhTien = cthd.SoLuong * spct.GiaBan;
            }
            // Cập nhật tổng tiền hóa đơn
            hoaDon.TongTien = await _context.ChiTietHoaDons.Where(x => x.IDHoaDon == dto.IDHoaDon).SumAsync(x => x.ThanhTien);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // 4. Chọn khách hàng (tìm theo SĐT)
        [HttpPost("chon-khach-hang")]
        public async Task<IActionResult> ChonKhachHang([FromBody] ChonKhachHangDto dto)
        {
            var kh = await _context.KhachHang.FirstOrDefaultAsync(x => x.SoDienThoai == dto.SoDienThoai);
            if (kh == null)
                return NotFound("Không tìm thấy khách hàng với số điện thoại này.");
            return Ok(new { kh.IDKhachHang, kh.TenKhachHang, kh.SoDienThoai });
        }

        // 5. Tạo khách hàng mới nhanh
        [HttpPost("tao-khach-hang")]
        public async Task<IActionResult> TaoKhachHang([FromBody] TaoKhachHangDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenKhachHang) || string.IsNullOrWhiteSpace(dto.SoDienThoai))
                return BadRequest("Tên và số điện thoại không được để trống.");
            var existed = await _context.KhachHang.AnyAsync(x => x.SoDienThoai == dto.SoDienThoai);
            if (existed)
                return BadRequest("Số điện thoại đã tồn tại.");
            var kh = new KhachHang
            {
                IDKhachHang = Guid.NewGuid(),
                MaKhachHang = $"KH{DateTime.Now:yyyyMMddHHmmssfff}",
                TenKhachHang = dto.TenKhachHang,
                SoDienThoai = dto.SoDienThoai,
                NgayTao = DateTime.Now,
                TrangThai = true
            };
            _context.KhachHang.Add(kh);
            await _context.SaveChangesAsync();
            return Ok(new { kh.IDKhachHang, kh.TenKhachHang, kh.SoDienThoai });
        }

        [HttpGet("danh-sach-san-pham")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.SanPhamChiTiets
                .Include(x => x.SanPham)
                    .ThenInclude(s => s.AnhSanPhams.Where(a => a.TrangThai))
                .Include(x => x.KichCo)
                .Include(x => x.MauSac)
                .Where(x => x.TrangThai)
                .Select(x => new {
                    id = x.IDSanPhamChiTiet,
                    name = x.SanPham.TenSanPham + $" [{x.KichCo.TenKichCo} - {x.MauSac.TenMauSac}]",
                    price = x.GiaBan,
                    size = x.KichCo.TenKichCo,
                    color = x.MauSac.TenMauSac,
                    // Lấy ảnh chính hoặc ảnh đầu tiên
                    img = x.SanPham.AnhSanPhams
                        .Where(a => a.TrangThai)
                        .OrderByDescending(a => a.LaAnhChinh)
                        .ThenBy(a => a.NgayTao)
                        .Select(a => a.UrlAnh)
                        .FirstOrDefault() ?? "/img/default-product.jpg",
                    stock = x.SoLuong,
                    // Thêm thông tin sản phẩm gốc
                    productId = x.IDSanPham,
                    productName = x.SanPham.TenSanPham,
                    // Thêm danh sách ảnh
                    images = x.SanPham.AnhSanPhams
                        .Where(a => a.TrangThai)
                        .OrderByDescending(a => a.LaAnhChinh)
                        .ThenBy(a => a.NgayTao)
                        .Select(a => new {
                            id = a.IDAnhSanPham,
                            url = a.UrlAnh,
                            isMain = a.LaAnhChinh
                        }).ToList()
                }).ToListAsync();
            return Ok(products);
        }

        [HttpGet("danh-sach-khach-hang")]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _context.KhachHang
                .Where(x => x.TrangThai)
                .Select(x => new {
                    id = x.IDKhachHang,
                    name = x.TenKhachHang,
                    email = x.Email,
                    phone = x.SoDienThoai,
                    point = 0, // Nếu có trường điểm thì thay thế
                    img = "/img/default-user.png" // Nếu có trường ảnh thì thay thế
                }).ToListAsync();
            return Ok(customers);
        }

        [HttpGet("tim-kiem-khach-hang")]
        public async Task<IActionResult> SearchCustomer(string query)
        {
            var customers = await _context.KhachHang
                .Where(x => x.TenKhachHang.Contains(query) || x.SoDienThoai.Contains(query))
                .Select(x => new {
                    id = x.IDKhachHang,
                    name = x.TenKhachHang,
                    email = x.Email,
                    phone = x.SoDienThoai,
                    point = 0, // Nếu có trường điểm thì thay thế
                    img = "/img/default-user.png"
                }).ToListAsync();
            return Ok(customers);
        }

        [HttpPost("them-khach-hang")]
        public async Task<IActionResult> AddCustomer([FromBody] TaoKhachHangDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenKhachHang) || string.IsNullOrWhiteSpace(dto.SoDienThoai))
                return BadRequest("Tên và số điện thoại không được để trống.");
            var existed = await _context.KhachHang.AnyAsync(x => x.SoDienThoai == dto.SoDienThoai);
            if (existed)
                return BadRequest("Số điện thoại đã tồn tại.");
            var kh = new KhachHang
            {
                IDKhachHang = Guid.NewGuid(),
                MaKhachHang = $"KH{DateTime.Now:yyyyMMddHHmmssfff}",
                TenKhachHang = dto.TenKhachHang,
                SoDienThoai = dto.SoDienThoai,
                NgayTao = DateTime.Now,
                TrangThai = true
            };
            _context.KhachHang.Add(kh);
            await _context.SaveChangesAsync();
            return Ok(new { kh.IDKhachHang, kh.TenKhachHang, kh.SoDienThoai, kh.Email });
        }

        [HttpGet("kiem-tra-ma-giam-gia")]
        public async Task<IActionResult> CheckDiscount(string code)
        {
            var discount = await _context.PhieuGiamGias
                .FirstOrDefaultAsync(x => x.MaCode == code && x.TrangThai);
            if (discount == null) return Ok(new { success = false });
            return Ok(new { success = true, value = discount.GiaTriGiam, max = discount.GiaTriGiamToiDa });
        }

        public class InvoiceProductDto
        {
            public Guid ProductDetailId { get; set; }
            public int Quantity { get; set; }
        }

        public class InvoiceDto
        {
            public Guid? CustomerId { get; set; }
            public string? CustomerName { get; set; }
            public string? CustomerPhone { get; set; }
            public string? CustomerEmail { get; set; }
            public string? Address { get; set; }
            public List<InvoiceProductDto> Products { get; set; }
            public string? DiscountCode { get; set; }
            public bool UsePoint { get; set; }
            public bool Shipping { get; set; }
            public string? PaymentMethod { get; set; } // Mã phương thức thanh toán ("cash", "bank", ...)
            public decimal? CustomerPaid { get; set; }
        }

        [HttpPost("thanh-toan")]
        public async Task<IActionResult> PayInvoice([FromBody] InvoiceDto dto)
        {
            Console.WriteLine($"[PayInvoice] PaymentMethod from client: {dto.PaymentMethod}");
            // Map code sang ID phương thức thanh toán
            Guid paymentMethodId = Guid.Empty;
            if (!string.IsNullOrEmpty(dto.PaymentMethod))
            {
                var method = await _context.PhuongThucThanhToans
                    .FirstOrDefaultAsync(x => x.MaPhuongThuc == dto.PaymentMethod && x.TrangThai);
                Console.WriteLine($"[PayInvoice] Query method result: {(method == null ? "null" : method.IDPhuongThucThanhToan.ToString())}");
                if (method == null)
                {
                    Console.WriteLine("[PayInvoice] ERROR: Phương thức thanh toán không hợp lệ.");
                    return BadRequest("Phương thức thanh toán không hợp lệ.");
                }
                paymentMethodId = method.IDPhuongThucThanhToan;
            }
            else
            {
                Console.WriteLine("[PayInvoice] ERROR: Chưa chọn phương thức thanh toán.");
                return BadRequest("Chưa chọn phương thức thanh toán.");
            }

            // 1. Tạo hóa đơn
            var hoaDon = new HoaDon
            {
                IDHoaDon = Guid.NewGuid(),
                MaHoaDon = $"HD{DateTime.Now:yyyyMMddHHmmssfff}",
                IDKhachHang = dto.CustomerId,
                TenNguoiNhan = dto.CustomerName,
                SoDienThoaiNguoiNhan = dto.CustomerPhone,
                DiaChiGiaoHang = dto.Address,
                TongTien = 0,
                TrangThai = "DaThanhToan",
                NgayTao = DateTime.Now,
                TrangThaiHoaDon = true,
                IDPhuongThucThanhToan = paymentMethodId
            };
            Console.WriteLine($"[PayInvoice] Create HoaDon: {hoaDon.MaHoaDon}, Customer: {hoaDon.TenNguoiNhan}, PaymentMethodId: {hoaDon.IDPhuongThucThanhToan}");
            _context.HoaDons.Add(hoaDon);

            // 2. Thêm chi tiết hóa đơn và kiểm tra tồn kho
            foreach (var p in dto.Products)
            {
                var spct = await _context.SanPhamChiTiets.FindAsync(p.ProductDetailId);
                if (spct == null) {
                    Console.WriteLine($"[PayInvoice] ERROR: Sản phẩm không tồn tại: {p.ProductDetailId}");
                    return BadRequest("Sản phẩm không tồn tại");
                }
                if (p.Quantity > spct.SoLuong) {
                    Console.WriteLine($"[PayInvoice] ERROR: Sản phẩm {spct.IDSanPhamChiTiet} vượt quá tồn kho ({spct.SoLuong})");
                    return BadRequest($"Sản phẩm {spct.IDSanPhamChiTiet} vượt quá tồn kho ({spct.SoLuong})");
                }
                var cthd = new ChiTietHoaDon
                {
                    IDChiTietHoaDon = Guid.NewGuid(),
                    MaChiTietHoaDon = $"CTHD{DateTime.Now:yyyyMMddHHmmssfff}",
                    IDHoaDon = hoaDon.IDHoaDon,
                    IDSanPhamChiTiet = p.ProductDetailId,
                    SoLuong = p.Quantity,
                    DonGia = spct.GiaBan,
                    ThanhTien = spct.GiaBan * p.Quantity,
                    NgayTao = DateTime.Now,
                    TrangThai = true
                };
                Console.WriteLine($"[PayInvoice] Add Product: {spct.IDSanPhamChiTiet}, Qty: {p.Quantity}, Price: {spct.GiaBan}");
                hoaDon.TongTien += cthd.ThanhTien;
                _context.ChiTietHoaDons.Add(cthd);

                // Trừ tồn kho
                spct.SoLuong -= p.Quantity;
            }

            // 3. Áp dụng mã giảm giá nếu có
            if (!string.IsNullOrEmpty(dto.DiscountCode))
            {
                var discount = await _context.PhieuGiamGias.FirstOrDefaultAsync(x => x.MaCode == dto.DiscountCode && x.TrangThai);
                if (discount != null)
                {
                    // Kiểm tra xem khách hàng có phiếu này không và còn số lượng không
                    if (dto.CustomerId.HasValue)
                    {
                        var customerVoucher = await _context.KhachHangPhieuGiams
                            .FirstOrDefaultAsync(x => x.IDPhieuGiamGia == discount.IDPhieuGiamGia && 
                                                     x.IDKhachHang == dto.CustomerId.Value &&
                                                     x.TrangThai &&
                                                     x.SoLuongDaSuDung < x.SoLuong);

                        if (customerVoucher != null)
                        {
                            // Tăng số lượng đã sử dụng
                            customerVoucher.SoLuongDaSuDung++;
                            customerVoucher.LanCapNhatCuoi = DateTime.UtcNow;
                            customerVoucher.NguoiCapNhat = "System";

                            // Nếu đã sử dụng hết, vô hiệu hóa
                            if (customerVoucher.SoLuongDaSuDung >= customerVoucher.SoLuong)
                            {
                                customerVoucher.TrangThai = false;
                            }

                            Console.WriteLine($"[PayInvoice] Use voucher: {discount.MaCode}, Used: {customerVoucher.SoLuongDaSuDung}/{customerVoucher.SoLuong}");
                        }
                        else
                        {
                            Console.WriteLine($"[PayInvoice] Customer voucher not found or used up: {dto.DiscountCode}");
                            return BadRequest("Phiếu giảm giá không hợp lệ hoặc đã được sử dụng hết.");
                        }
                    }

                    hoaDon.TienGiam = discount.GiaTriGiam;
                    hoaDon.IDPhieuGiamGia = discount.IDPhieuGiamGia;
                    hoaDon.TongTien -= discount.GiaTriGiam;
                    Console.WriteLine($"[PayInvoice] Apply Discount: {discount.MaCode}, Value: {discount.GiaTriGiam}");
                }
                else
                {
                    Console.WriteLine($"[PayInvoice] Discount code not found or inactive: {dto.DiscountCode}");
                    return BadRequest("Mã giảm giá không hợp lệ.");
                }
            }

            // 4. (Tùy chọn) Lưu CustomerPaid vào ghi chú hoặc trường phù hợp nếu muốn
            // hoaDon.GhiChu = $"Khách đưa: {dto.CustomerPaid}";

            await _context.SaveChangesAsync();
            Console.WriteLine($"[PayInvoice] SUCCESS: HoaDon {hoaDon.MaHoaDon} created.");
            return Ok(new { hoaDon.IDHoaDon, hoaDon.MaHoaDon });
        }

        [HttpGet("danh-sach-phuong-thuc-thanh-toan")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var methods = await _context.PhuongThucThanhToans
                .Where(x => x.TrangThai)
                .Select(x => new { x.IDPhuongThucThanhToan, x.MaPhuongThuc, x.TenPhuongThuc })
                .ToListAsync();
            return Ok(methods);
        }

        [HttpGet("danh-sach-phieu-giam-gia-khach-hang")]
        public async Task<IActionResult> GetCustomerDiscountVouchers(Guid customerId)
        {
            var vouchers = await _context.KhachHangPhieuGiams
                .Include(k => k.PhieuGiamGia)
                .Where(x => x.IDKhachHang == customerId && 
                           x.TrangThai && 
                           x.PhieuGiamGia.TrangThai &&
                           x.SoLuongDaSuDung < x.SoLuong && // Chỉ lấy phiếu còn số lượng
                           x.PhieuGiamGia.NgayBatDau <= DateTime.UtcNow && // Kiểm tra thời gian hiệu lực
                           x.PhieuGiamGia.NgayKetThuc >= DateTime.UtcNow)
                .Select(x => new {
                    id = x.IDPhieuGiamGia,
                    maCode = x.PhieuGiamGia.MaCode,
                    tenPhieu = x.PhieuGiamGia.TenPhieu,
                    giaTriGiam = x.PhieuGiamGia.GiaTriGiam,
                    giaTriGiamToiDa = x.PhieuGiamGia.GiaTriGiamToiDa,
                    donToiThieu = x.PhieuGiamGia.DonToiThieu,
                    ngayBatDau = x.PhieuGiamGia.NgayBatDau,
                    ngayKetThuc = x.PhieuGiamGia.NgayKetThuc,
                    soLuong = x.SoLuong,
                    soLuongDaSuDung = x.SoLuongDaSuDung,
                    soLuongConLai = x.SoLuong - x.SoLuongDaSuDung
                })
                .ToListAsync();
            return Ok(vouchers);
        }

        // Lấy chi tiết sản phẩm với đầy đủ ảnh
        [HttpGet("chi-tiet-san-pham/{id}")]
        public async Task<IActionResult> GetProductDetail(Guid id)
        {
            var product = await _context.SanPhamChiTiets
                .Include(x => x.SanPham)
                    .ThenInclude(s => s.AnhSanPhams.Where(a => a.TrangThai))
                .Include(x => x.SanPham)
                    .ThenInclude(s => s.ChatLieu)
                .Include(x => x.SanPham)
                    .ThenInclude(s => s.DanhMuc)
                .Include(x => x.SanPham)
                    .ThenInclude(s => s.ThuongHieu)
                .Include(x => x.KichCo)
                .Include(x => x.MauSac)
                .Include(x => x.HoaTiet)
                .Where(x => x.IDSanPhamChiTiet == id && x.TrangThai)
                .Select(x => new {
                    id = x.IDSanPhamChiTiet,
                    productId = x.IDSanPham,
                    name = x.SanPham.TenSanPham,
                    description = $"Kích cỡ: {x.KichCo.TenKichCo}, Màu sắc: {x.MauSac.TenMauSac}" + 
                                 (x.HoaTiet != null ? $", Họa tiết: {x.HoaTiet.TenHoaTiet}" : ""),
                    price = x.GiaBan,
                    stock = x.SoLuong,
                    size = x.KichCo.TenKichCo,
                    color = x.MauSac.TenMauSac,
                    pattern = x.HoaTiet != null ? x.HoaTiet.TenHoaTiet : null,
                    // Thông tin sản phẩm gốc
                    material = x.SanPham.ChatLieu.TenChatLieu,
                    category = x.SanPham.DanhMuc.TenDanhMuc,
                    brand = x.SanPham.ThuongHieu.TenThuongHieu,
                    hasPleats = x.SanPham.CoXepLy,
                    hasElastic = x.SanPham.CoGian,
                    // Ảnh chính
                    mainImage = x.SanPham.AnhSanPhams
                        .Where(a => a.TrangThai)
                        .OrderByDescending(a => a.LaAnhChinh)
                        .ThenBy(a => a.NgayTao)
                        .Select(a => a.UrlAnh)
                        .FirstOrDefault() ?? "/img/default-product.jpg",
                    // Danh sách tất cả ảnh
                    images = x.SanPham.AnhSanPhams
                        .Where(a => a.TrangThai)
                        .OrderByDescending(a => a.LaAnhChinh)
                        .ThenBy(a => a.NgayTao)
                        .Select(a => new {
                            id = a.IDAnhSanPham,
                            url = a.UrlAnh,
                            isMain = a.LaAnhChinh,
                            createdAt = a.NgayTao
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound("Không tìm thấy sản phẩm.");

            return Ok(product);
        }
    }
} 