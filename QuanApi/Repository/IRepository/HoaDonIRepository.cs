using QuanApi.Data;
using System;
using System.Collections.Generic;

namespace QuanApi.Repository.IRepository
{
    public interface HoaDonIRepository
    {
        List<HoaDon> GetAll();
        HoaDon? GetById(Guid id);
        bool CreateHoaDon(HoaDon hoaDon);
        bool UpdateTrangThai(Guid id, string trangThai);
        bool DeleteHoaDon(Guid idHoaDon);

        bool ThemChiTietHoaDon(Guid idHoaDon, Guid idSanPhamChiTiet, int soLuong, decimal donGia);
        bool CapNhatSoLuongChiTiet(Guid idChiTiet, int soLuongMoi, decimal donGia);
        bool XoaChiTietHoaDon(Guid idChiTiet);
        List<ChiTietHoaDon> LayChiTietTheoHoaDon(Guid idHoaDon);

        bool CapNhatKhachHang(Guid idHoaDon, Guid idKhachHang);
        bool ThanhToanHoaDon(Guid idHoaDon, string maPhuongThucThanhToan);
    }
}
