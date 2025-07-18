using QuanApi.Data;
using QuanApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using BanQuanAu1.Web.Data;

namespace QuanApi.Repository
{
    public class HoaDonRepository : HoaDonIRepository
    {
        private readonly BanQuanAu1DbContext _context;

        public HoaDonRepository(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        public List<HoaDon> GetAll()
        {
            return _context.HoaDons.Include(h => h.ChiTietHoaDons).ToList();
        }

        public HoaDon? GetById(Guid id)
        {
            return _context.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .FirstOrDefault(h => h.IDHoaDon == id);
        }

        public bool CreateHoaDon(HoaDon hoaDon)
        {
            try
            {
                _context.HoaDons.Add(hoaDon);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateTrangThai(Guid id, string trangThai)
        {
            try
            {
                var hd = _context.HoaDons.Find(id);
                if (hd == null) return false;
                hd.TrangThai = trangThai;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteHoaDon(Guid idHoaDon)
        {
            try
            {
                var chiTiets = _context.ChiTietHoaDons.Where(ct => ct.IDHoaDon == idHoaDon).ToList();
                _context.ChiTietHoaDons.RemoveRange(chiTiets);

                var hoaDon = _context.HoaDons.Find(idHoaDon);
                if (hoaDon != null)
                {
                    _context.HoaDons.Remove(hoaDon);
                }

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ThemChiTietHoaDon(Guid idHoaDon, Guid idSanPhamChiTiet, int soLuong, decimal donGia)
        {
            try
            {
                var sanPham = _context.SanPhamChiTiets.Find(idSanPhamChiTiet);
                if (sanPham == null || sanPham.SoLuong < soLuong) return false;

                var ct = _context.ChiTietHoaDons.FirstOrDefault(c =>
                    c.IDHoaDon == idHoaDon && c.IDSanPhamChiTiet == idSanPhamChiTiet);

                if (ct != null)
                {
                    ct.SoLuong += soLuong;
                    ct.ThanhTien = ct.SoLuong * donGia;
                    ct.LanCapNhatCuoi = DateTime.Now;
                }
                else
                {
                    ct = new ChiTietHoaDon
                    {
                        IDHoaDon = idHoaDon,
                        IDSanPhamChiTiet = idSanPhamChiTiet,
                        SoLuong = soLuong,
                        DonGia = donGia,
                        ThanhTien = soLuong * donGia,
                        NgayTao = DateTime.Now,
                        TrangThai = true
                    };
                    _context.ChiTietHoaDons.Add(ct);
                }

                sanPham.SoLuong -= soLuong;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CapNhatSoLuongChiTiet(Guid idChiTiet, int soLuongMoi, decimal donGia)
        {
            try
            {
                var ct = _context.ChiTietHoaDons.Find(idChiTiet);
                if (ct == null) return false;

                var sanPham = _context.SanPhamChiTiets.Find(ct.IDSanPhamChiTiet);
                if (sanPham == null) return false;

                int chenhlech = soLuongMoi - ct.SoLuong;
                if (sanPham.SoLuong < chenhlech) return false;

                sanPham.SoLuong -= chenhlech;
                ct.SoLuong = soLuongMoi;
                ct.DonGia = donGia;
                ct.ThanhTien = soLuongMoi * donGia;
                ct.LanCapNhatCuoi = DateTime.Now;

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool XoaChiTietHoaDon(Guid idChiTiet)
        {
            try
            {
                var ct = _context.ChiTietHoaDons.Find(idChiTiet);
                if (ct == null) return false;

                var sanPham = _context.SanPhamChiTiets.Find(ct.IDSanPhamChiTiet);
                if (sanPham != null)
                {
                    sanPham.SoLuong += ct.SoLuong;
                }

                _context.ChiTietHoaDons.Remove(ct);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<ChiTietHoaDon> LayChiTietTheoHoaDon(Guid idHoaDon)
        {
            return _context.ChiTietHoaDons
                .Where(ct => ct.IDHoaDon == idHoaDon)
                .Include(ct => ct.SanPhamChiTiet)
                .ToList();
        }

        public bool CapNhatKhachHang(Guid idHoaDon, Guid idKhachHang)
        {
            try
            {
                var hoaDon = _context.HoaDons.Find(idHoaDon);
                if (hoaDon == null) return false;

                hoaDon.IDKhachHang = idKhachHang;
                hoaDon.LanCapNhatCuoi = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ThanhToanHoaDon(Guid idHoaDon, string maPhuongThucThanhToan)
        {
            try
            {
                var hoaDon = _context.HoaDons.Find(idHoaDon);
                if (hoaDon == null) return false;

                hoaDon.TrangThai = "Đã thanh toán";

                var pttt = _context.PhuongThucThanhToans
                    .FirstOrDefault(p => p.MaPhuongThuc == maPhuongThucThanhToan);

                if (pttt == null)
                {
                    return false; 
                }

                hoaDon.IDPhuongThucThanhToan = pttt.IDPhuongThucThanhToan;

                hoaDon.LanCapNhatCuoi = DateTime.Now;

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
