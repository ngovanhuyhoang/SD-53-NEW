using QuanApi.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanApi.Repository.IRepository
{
    public interface HoaDonIRepository
    {
        Task<HoaDon> TaoHoaDonAsync(HoaDon hoaDon);

        Task<IEnumerable<HoaDon>> GetAllHoaDonAsync();

        Task<HoaDon?> GetHoaDonByIdAsync(Guid id);

        Task<bool> ThemSanPhamVaoHoaDonAsync(Guid hoaDonId, ChiTietHoaDon chiTiet);

        Task<bool> XoaSanPhamKhoiHoaDonAsync(Guid hoaDonId, Guid chiTietId);

        Task<bool> CapNhatKhachHangAsync(Guid hoaDonId, Guid khachHangId);

        Task<bool> ThanhToanHoaDonAsync(Guid hoaDonId);
    }
}
