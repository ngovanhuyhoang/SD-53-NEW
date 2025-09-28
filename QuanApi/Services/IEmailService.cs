using System.Threading.Tasks;
using QuanApi.Data;

namespace QuanApi.Services
{
    public interface IEmailService
    {
        Task SendOrderStatusChangeEmailAsync(HoaDon hoaDon, string oldStatus, string newStatus);
        Task SendOrderCancellationEmailAsync(HoaDon hoaDon, string reason);
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendEmployeeCredentialsEmailAsync(string email, string employeeName, string employeeCode, string password);
    }
}
