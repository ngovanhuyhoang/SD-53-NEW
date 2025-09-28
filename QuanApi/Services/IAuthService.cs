using QuanApi.Data;
using QuanApi.Dtos;

namespace QuanApi.Services
{
    public interface IAuthService
    {
        Task<NhanVien?> AuthenticateAsync(string email, string password);
        Task<bool> ValidatePasswordAsync(Guid employeeId, string password);
    }
}
