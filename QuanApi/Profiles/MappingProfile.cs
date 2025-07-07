using AutoMapper;
using QuanApi.Data;
using QuanApi.Dtos;

namespace QuanApi.MappingProfiles
{
    public class KhachHangMappingProfile : Profile
    {
        public KhachHangMappingProfile()
        {
            CreateMap<KhachHang, KhachHangDto>();

            CreateMap<CreateKhachHangDto, KhachHang>()
                .ForMember(dest => dest.IDKhachHang, opt => opt.Ignore())
                .ForMember(dest => dest.NgayTao, opt => opt.Ignore())
                .ForMember(dest => dest.NguoiTao, opt => opt.Ignore())
                .ForMember(dest => dest.LanCapNhatCuoi, opt => opt.Ignore())
                .ForMember(dest => dest.NguoiCapNhat, opt => opt.Ignore())
                .ForMember(dest => dest.MatKhau, opt => opt.Ignore());

            CreateMap<UpdateKhachHangDto, KhachHang>()
                .ForMember(dest => dest.NgayTao, opt => opt.Ignore())
                .ForMember(dest => dest.NguoiTao, opt => opt.Ignore())
                .ForMember(dest => dest.LanCapNhatCuoi, opt => opt.Ignore())
                .ForMember(dest => dest.NguoiCapNhat, opt => opt.Ignore())
                .ForMember(dest => dest.MatKhau, opt => opt.Ignore());
        }
    }
}
