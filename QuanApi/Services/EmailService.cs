using Microsoft.Extensions.Options;
using QuanApi.Models;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace QuanApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                {
                    client.EnableSsl = _emailSettings.EnableSsl;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Email đã gửi thành công tới {toEmail} với chủ đề: {subject}");
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, $"Lỗi SMTP khi gửi email tới {toEmail}. Mã lỗi: {smtpEx.StatusCode}. Chi tiết: {smtpEx.Message}");
                throw new ApplicationException($"Lỗi cấu hình hoặc kết nối SMTP: {smtpEx.Message}", smtpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi không xác định khi gửi email tới {toEmail}. Chi tiết: {ex.Message}");
                throw new ApplicationException($"Lỗi khi gửi email: {ex.Message}", ex);
            }
        }
    }
}