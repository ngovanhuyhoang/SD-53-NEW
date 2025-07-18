using Microsoft.AspNetCore.Mvc;
using QuanApi.Data;
using QuanApi.Repository.IRepository;
using System;
using System.Threading.Tasks;

namespace QuanApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BanHangTaiQuaysController : ControllerBase
    {
        private readonly HoaDonIRepository _hoaDonRepo;

        public BanHangTaiQuaysController(HoaDonIRepository hoaDonRepo)
        {
            _hoaDonRepo = hoaDonRepo;
        }

        [HttpPost("TaoHoaDon")]
        public IActionResult TaoHoaDon()
        {
            var hoaDon = new HoaDon
            {
                IDHoaDon = Guid.NewGuid(),
                NgayTao = DateTime.Now,
                TrangThai = "Chờ thanh toán",
                IDKhachHang = null
            };

            var result = _hoaDonRepo.CreateHoaDon(hoaDon);
            return result ? Ok(hoaDon) : BadRequest("Tạo thất bại");
        }



        [HttpPost("ThemSanPham")]
        public IActionResult ThemSanPham(Guid idHoaDon, Guid idSanPhamChiTiet, int soLuong, decimal donGia)
        {
            var result = _hoaDonRepo.ThemChiTietHoaDon(idHoaDon, idSanPhamChiTiet, soLuong, donGia);
            return result ? Ok("Thêm sản phẩm thành công") : BadRequest("Thêm thất bại");
        }

        [HttpPut("CapNhatSoLuong")]
        public IActionResult CapNhatSoLuong(Guid idChiTiet, int soLuongMoi, decimal donGia)
        {
            var result = _hoaDonRepo.CapNhatSoLuongChiTiet(idChiTiet, soLuongMoi, donGia);
            return result ? Ok("Cập nhật thành công") : BadRequest("Cập nhật thất bại");
        }

        [HttpDelete("XoaSanPham/{idChiTiet}")]
        public IActionResult XoaSanPham(Guid idChiTiet)
        {
            var result = _hoaDonRepo.XoaChiTietHoaDon(idChiTiet);
            return result ? Ok("Đã xóa sản phẩm") : BadRequest("Xóa thất bại");
        }

        [HttpGet("LayChiTiet/{idHoaDon}")]
        public IActionResult LayChiTiet(Guid idHoaDon)
        {
            var result = _hoaDonRepo.LayChiTietTheoHoaDon(idHoaDon);
            return Ok(result);
        }

        [HttpPut("CapNhatKhachHang")]
        public IActionResult CapNhatKhachHang(Guid idHoaDon, Guid idKhachHang)
        {
            var result = _hoaDonRepo.CapNhatKhachHang(idHoaDon, idKhachHang);
            return result ? Ok("Cập nhật khách hàng thành công") : BadRequest("Thất bại");
        }

        [HttpPut("ThanhToan")]
        public IActionResult ThanhToan(Guid idHoaDon, string maPhuongThucThanhToan)
        {
            var result = _hoaDonRepo.ThanhToanHoaDon(idHoaDon, maPhuongThucThanhToan);
            return result ? Ok("Thanh toán thành công") : BadRequest("Thanh toán thất bại");
        }
    }
}
