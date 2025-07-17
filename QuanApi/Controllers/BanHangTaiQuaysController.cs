using Microsoft.AspNetCore.Mvc;
using QuanApi.Repository.IRepository;
using QuanApi.Data;
using System;
using System.Threading.Tasks;
using QuanApi.Repository;
using QuanApi.Dtos;

namespace QuanApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BanHangTaiQuaysController : ControllerBase
    {
        private readonly HoaDonIRepository _hd;

        public BanHangTaiQuaysController(HoaDonIRepository hd)
        {
            _hd = hd;
        }

        [HttpPost("TaoHoaDon")]
        public async Task<IActionResult> TaoHoaDon()
        {
            var hoaDon = new HoaDon
            {
                IDKhachHang = null, 
                IDNhanVien = null,  
                MaHoaDon = $"HD-{DateTime.Now:yyyyMMddHHmmss}",
                NgayTao = DateTime.Now,
                TongTien = 0,
                TrangThai = "ChuaThanhToan",
                TrangThaiHoaDon = true
            };

            var result = await _hd.TaoHoaDonAsync(hoaDon);
            return Ok(result);
        }





        // GET: api/BanHangTaiQuays
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _hd.GetAllHoaDonAsync();

            if (result == null)
            {
                return Ok(new List<HoaDon>());
            }

            return Ok(result);
        }

        // GET: api/BanHangTaiQuays/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var hoaDon = await _hd.GetHoaDonByIdAsync(id);
            if (hoaDon == null)
                return NotFound();
            return Ok(hoaDon);
        }

        // POST: api/BanHangTaiQuays/ThemSanPham
        [HttpPost("ThemSanPham")]
        public async Task<IActionResult> ThemSanPham([FromQuery] Guid hoaDonId, [FromBody] ChiTietHoaDon chiTiet)
        {
            var result = await _hd.ThemSanPhamVaoHoaDonAsync(hoaDonId, chiTiet);
            return result ? Ok("Đã thêm sản phẩm") : BadRequest("Thêm sản phẩm thất bại");
        }

        // DELETE: api/BanHangTaiQuays/XoaSanPham?hoaDonId=...&chiTietId=...
        [HttpDelete("XoaSanPham")]
        public async Task<IActionResult> XoaSanPham(Guid hoaDonId, Guid chiTietId)
        {
            var result = await _hd.XoaSanPhamKhoiHoaDonAsync(hoaDonId, chiTietId);
            return result ? Ok("Đã xoá sản phẩm") : BadRequest("Xoá thất bại");
        }

        // PUT: api/BanHangTaiQuays/CapNhatKhachHang?hoaDonId=...&khachHangId=...
        [HttpPut("CapNhatKhachHang")]
        public async Task<IActionResult> CapNhatKhachHang(Guid hoaDonId, Guid khachHangId)
        {
            var result = await _hd.CapNhatKhachHangAsync(hoaDonId, khachHangId);
            return result ? Ok("Đã cập nhật khách hàng") : BadRequest("Cập nhật thất bại");
        }

        // PUT: api/BanHangTaiQuays/ThanhToan?hoaDonId=...
        [HttpPut("ThanhToan")]
        public async Task<IActionResult> ThanhToan(Guid hoaDonId)
        {
            var result = await _hd.ThanhToanHoaDonAsync(hoaDonId);
            return result ? Ok("Đã thanh toán") : BadRequest("Thanh toán thất bại");
        }
    }
}
