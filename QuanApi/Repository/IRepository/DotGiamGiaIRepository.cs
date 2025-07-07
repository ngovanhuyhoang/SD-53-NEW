using BanQuanAu1.Web.Data;
using QuanApi.Data;
using QuanApi.Dtos;

namespace QuanApi.Repository.IRepository
{
    public interface DotGiamGiaIRepository
    {
        Task<PagedResultGeneric<DotGiamGia>> GetDotGiamGia(string maDot, string tenDot, int? phanTramGiam,
                    DateTime? tuNgay, DateTime? denNgay, string trangThai, int page, int pageSize);

        Task<DotGiamGia> GetByIdAsync(Guid id);

        Task<bool> CreateAsync(DotGiamGia dotGiamGia, List<Guid> selectedChiTietIds);

        Task<bool> UpdateAsync(DotGiamGia dotGiamGia);

        Task<bool> DeleteAsync(Guid id);

        Task<bool> UpdateTrangThaiAsync(Guid id, bool trangThai);
    }
}
