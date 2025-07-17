using QuanApi.Data;
using QuanApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using BanQuanAu1.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanApi.Repository
{
    public class HoaDonRepository : HoaDonIRepository
    {
        private readonly BanQuanAu1DbContext _context;

        public HoaDonRepository(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        public async Task<HoaDon> TaoHoaDonAsync(HoaDon hoaDon)
        {
            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();
            return hoaDon;
        }


        public async Task<HoaDon?> GetHoaDonByIdAsync(Guid id)
        {
            return await _context.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .ThenInclude(c => c.SanPhamChiTiet)
                .FirstOrDefaultAsync(h => h.IDHoaDon == id && h.TrangThaiHoaDon);
        }

        public async Task<IEnumerable<HoaDon>> GetAllHoaDonAsync()
        {
            return await _context.HoaDons
                .Where(h => h.TrangThaiHoaDon)
                .Include(h => h.ChiTietHoaDons)
                .ToListAsync();
        }

        public async Task<bool> ThemSanPhamVaoHoaDonAsync(Guid hoaDonId, ChiTietHoaDon chiTiet)
        {
            var hoaDon = await _context.HoaDons.FindAsync(hoaDonId);
            if (hoaDon == null || !hoaDon.TrangThaiHoaDon) return false;

            chiTiet.IDChiTietHoaDon = Guid.NewGuid();
            chiTiet.MaChiTietHoaDon = $"CTHD-{DateTime.Now:yyyyMMddHHmmss}";
            chiTiet.IDHoaDon = hoaDonId;
            chiTiet.ThanhTien = chiTiet.SoLuong * chiTiet.DonGia;
            chiTiet.NgayTao = DateTime.Now;

            _context.ChiTietHoaDons.Add(chiTiet);

            hoaDon.TongTien += chiTiet.ThanhTien;
            _context.HoaDons.Update(hoaDon);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> XoaSanPhamKhoiHoaDonAsync(Guid hoaDonId, Guid chiTietId)
        {
            var chiTiet = await _context.ChiTietHoaDons
                .FirstOrDefaultAsync(c => c.IDChiTietHoaDon == chiTietId && c.IDHoaDon == hoaDonId && c.TrangThai);
            if (chiTiet == null) return false;

            var hoaDon = await _context.HoaDons.FindAsync(hoaDonId);
            if (hoaDon == null) return false;

            hoaDon.TongTien -= chiTiet.ThanhTien;
            _context.ChiTietHoaDons.Remove(chiTiet);
            _context.HoaDons.Update(hoaDon);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CapNhatKhachHangAsync(Guid hoaDonId, Guid khachHangId)
        {
            var hoaDon = await _context.HoaDons.FindAsync(hoaDonId);
            if (hoaDon == null) return false;

            hoaDon.IDKhachHang = khachHangId;
            hoaDon.LanCapNhatCuoi = DateTime.Now;

            _context.HoaDons.Update(hoaDon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ThanhToanHoaDonAsync(Guid hoaDonId)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .FirstOrDefaultAsync(h => h.IDHoaDon == hoaDonId && h.TrangThaiHoaDon);

            if (hoaDon == null) return false;

            if (hoaDon.TrangThai == TrangThaiHoaDon.DaThanhToan.ToString())
                return false;

            hoaDon.TrangThai = TrangThaiHoaDon.DaThanhToan.ToString();
            hoaDon.LanCapNhatCuoi = DateTime.Now;

            if (hoaDon.TongTien == 0 && hoaDon.ChiTietHoaDons != null)
            {
                hoaDon.TongTien = hoaDon.ChiTietHoaDons.Sum(x => x.ThanhTien);
            }

            if (hoaDon.TienGiam.HasValue)
            {
                hoaDon.TongTien -= hoaDon.TienGiam.Value;
                if (hoaDon.TongTien < 0) hoaDon.TongTien = 0;
            }

            _context.HoaDons.Update(hoaDon);
            await _context.SaveChangesAsync();
            return true;
        }


        public enum TrangThaiHoaDon
        {
            ChuaThanhToan,
            DaThanhToan
        }
    }
}
