using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using QuanApi.Data;
using Microsoft.EntityFrameworkCore;
using BanQuanAu1.Web.Data;
using QuanApi.Models;
using Microsoft.Extensions.Logging;

namespace QuanApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly BanQuanAu1DbContext _context;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, BanQuanAu1DbContext context, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public async Task SendOrderStatusChangeEmailAsync(HoaDon hoaDon, string oldStatus, string newStatus)
        {
            try
            {
                // Load đầy đủ thông tin đơn hàng
                var fullOrder = await _context.HoaDons
                    .Include(h => h.KhachHang)
                    .Include(h => h.ChiTietHoaDons)
                        .ThenInclude(ct => ct.SanPhamChiTiet)
                            .ThenInclude(sp => sp.SanPham)
                    .FirstOrDefaultAsync(h => h.IDHoaDon == hoaDon.IDHoaDon);

                if (fullOrder?.KhachHang?.Email == null)
                {
                    _logger.LogWarning($"Không thể gửi email: Khách hàng không có email cho đơn hàng {hoaDon.MaHoaDon}");
                    return;
                }

                var subject = GetEmailSubject(newStatus, hoaDon.MaHoaDon);
                var body = GenerateStatusChangeEmailBody(fullOrder, oldStatus, newStatus);

                await SendEmailAsync(fullOrder.KhachHang.Email, subject, body);

                _logger.LogInformation($"Đã gửi email thông báo thay đổi trạng thái từ '{oldStatus}' sang '{newStatus}' cho đơn hàng {hoaDon.MaHoaDon}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi gửi email thông báo trạng thái cho đơn hàng {hoaDon.MaHoaDon}: {ex.Message}");
            }
        }

        public async Task SendOrderCancellationEmailAsync(HoaDon hoaDon, string reason)
        {
            try
            {
                var fullOrder = await _context.HoaDons
                    .Include(h => h.KhachHang)
                    .Include(h => h.ChiTietHoaDons)
                        .ThenInclude(ct => ct.SanPhamChiTiet)
                            .ThenInclude(sp => sp.SanPham)
                    .FirstOrDefaultAsync(h => h.IDHoaDon == hoaDon.IDHoaDon);

                if (fullOrder?.KhachHang?.Email == null) return;

                var subject = $"Thông báo hủy đơn hàng #{hoaDon.MaHoaDon}";
                var body = GenerateCancellationEmailBody(fullOrder, reason);

                await SendEmailAsync(fullOrder.KhachHang.Email, subject, body);

                _logger.LogInformation($"Đã gửi email thông báo hủy đơn hàng {hoaDon.MaHoaDon}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi gửi email hủy đơn cho đơn hàng {hoaDon.MaHoaDon}: {ex.Message}");
                throw new ApplicationException($"Lỗi khi gửi email hủy đơn: {ex.Message}", ex);
            }
        }


        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromPassword = _configuration["EmailSettings:FromPassword"];
                var fromName = _configuration["EmailSettings:FromName"];

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email đã gửi thành công tới {toEmail} với chủ đề: {subject}");
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

        private string GetEmailSubject(string status, string orderCode)
        {
            return status switch
            {
                "Đã xác nhận" => $"Đơn hàng #{orderCode} đã được xác nhận",
                "Đang chuẩn bị" => $"Đơn hàng #{orderCode} đang được chuẩn bị",
                "Đang giao hàng" => $"Đơn hàng #{orderCode} đang được giao",
                "Đã giao hàng" => $"Đơn hàng #{orderCode} đã được giao thành công",
                "Đã hủy" => $"Đơn hàng #{orderCode} đã bị hủy",
                _ => $"Cập nhật trạng thái đơn hàng #{orderCode}"
            };
        }

        private string GenerateStatusChangeEmailBody(HoaDon hoaDon, string oldStatus, string newStatus)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset='UTF-8'></head><body>");
            sb.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>");

            // Header
            sb.AppendLine("<div style='background-color: #007bff; color: white; padding: 20px; text-align: center;'>");
            sb.AppendLine("<h1>Cập nhật trạng thái đơn hàng</h1>");
            sb.AppendLine("</div>");

            // Content
            sb.AppendLine("<div style='padding: 20px;'>");
            sb.AppendLine($"<p>Xin chào <strong>{hoaDon.KhachHang?.TenKhachHang ?? hoaDon.TenNguoiNhan}</strong>,</p>");
            sb.AppendLine($"<p>Đơn hàng <strong>#{hoaDon.MaHoaDon}</strong> của bạn đã được cập nhật trạng thái:</p>");

            // Status change
            sb.AppendLine("<div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 15px 0;'>");
            sb.AppendLine($"<p><strong>Trạng thái cũ:</strong> <span style='color: #6c757d;'>{oldStatus}</span></p>");
            sb.AppendLine($"<p><strong>Trạng thái mới:</strong> <span style='color: #28a745; font-weight: bold;'>{newStatus}</span></p>");
            sb.AppendLine($"<p><strong>Thời gian cập nhật:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            sb.AppendLine("</div>");

            // Order details
            sb.AppendLine("<h3>Chi tiết đơn hàng:</h3>");
            sb.AppendLine("<table style='width: 100%; border-collapse: collapse;'>");
            sb.AppendLine("<tr style='background-color: #f8f9fa;'>");
            sb.AppendLine("<th style='border: 1px solid #dee2e6; padding: 8px; text-align: left;'>Sản phẩm</th>");
            sb.AppendLine("<th style='border: 1px solid #dee2e6; padding: 8px; text-align: center;'>Số lượng</th>");
            sb.AppendLine("<th style='border: 1px solid #dee2e6; padding: 8px; text-align: right;'>Đơn giá</th>");
            sb.AppendLine("<th style='border: 1px solid #dee2e6; padding: 8px; text-align: right;'>Thành tiền</th>");
            sb.AppendLine("</tr>");

            foreach (var item in hoaDon.ChiTietHoaDons ?? new List<ChiTietHoaDon>())
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td style='border: 1px solid #dee2e6; padding: 8px;'>{item.SanPhamChiTiet?.SanPham?.TenSanPham}</td>");
                sb.AppendLine($"<td style='border: 1px solid #dee2e6; padding: 8px; text-align: center;'>{item.SoLuong}</td>");
                sb.AppendLine($"<td style='border: 1px solid #dee2e6; padding: 8px; text-align: right;'>{item.DonGia:N0} VNĐ</td>");
                sb.AppendLine($"<td style='border: 1px solid #dee2e6; padding: 8px; text-align: right;'>{item.ThanhTien:N0} VNĐ</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine($"<p style='text-align: right; font-weight: bold; font-size: 16px; margin-top: 10px;'>Tổng tiền: {hoaDon.TongTien:N0} VNĐ</p>");

            // Status-specific messages
            sb.AppendLine(GetStatusSpecificMessage(newStatus));

            sb.AppendLine("<p>Cảm ơn bạn đã mua sắm tại cửa hàng của chúng tôi!</p>");
            sb.AppendLine("</div>");

            // Footer
            sb.AppendLine("<div style='background-color: #f8f9fa; padding: 15px; text-align: center; color: #6c757d;'>");
            sb.AppendLine("<p>Đây là email tự động, vui lòng không trả lời email này.</p>");
            sb.AppendLine("<p>Nếu có thắc mắc, vui lòng liên hệ: support@example.com | 0123-456-789</p>");
            sb.AppendLine("</div>");

            sb.AppendLine("</div></body></html>");

            return sb.ToString();
        }

        private string GenerateCancellationEmailBody(HoaDon hoaDon, string reason)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset='UTF-8'></head><body>");
            sb.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>");

            // Header
            sb.AppendLine("<div style='background-color: #dc3545; color: white; padding: 20px; text-align: center;'>");
            sb.AppendLine("<h1>Thông báo hủy đơn hàng</h1>");
            sb.AppendLine("</div>");

            // Content
            sb.AppendLine("<div style='padding: 20px;'>");
            sb.AppendLine($"<p>Xin chào <strong>{hoaDon.KhachHang?.TenKhachHang ?? hoaDon.TenNguoiNhan}</strong>,</p>");
            sb.AppendLine($"<p>Chúng tôi rất tiếc phải thông báo rằng đơn hàng <strong>#{hoaDon.MaHoaDon}</strong> của bạn đã bị hủy.</p>");

            if (!string.IsNullOrEmpty(reason))
            {
                sb.AppendLine("<div style='background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 15px 0;'>");
                sb.AppendLine($"<p><strong>Lý do hủy:</strong> {reason}</p>");
                sb.AppendLine("</div>");
            }

            sb.AppendLine($"<p><strong>Thời gian hủy:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            sb.AppendLine($"<p><strong>Tổng tiền đơn hàng:</strong> {hoaDon.TongTien:N0} VNĐ</p>");

            sb.AppendLine("<p>Nếu bạn đã thanh toán, chúng tôi sẽ hoàn tiền trong vòng 3-7 ngày làm việc.</p>");
            sb.AppendLine("<p>Chúng tôi xin lỗi vì sự bất tiện này và hy vọng được phục vụ bạn trong tương lai.</p>");
            sb.AppendLine("</div>");

            // Footer
            sb.AppendLine("<div style='background-color: #f8f9fa; padding: 15px; text-align: center; color: #6c757d;'>");
            sb.AppendLine("<p>Nếu có thắc mắc, vui lòng liên hệ: support@example.com | 0123-456-789</p>");
            sb.AppendLine("</div>");

            sb.AppendLine("</div></body></html>");

            return sb.ToString();
        }

        private string GetStatusSpecificMessage(string status)
        {
            return status switch
            {
                "Đã xác nhận" => "<div style='background-color: #d4edda; border: 1px solid #c3e6cb; padding: 10px; border-radius: 5px; margin: 15px 0;'><p><strong>Đơn hàng của bạn đã được xác nhận!</strong> Chúng tôi sẽ chuẩn bị và giao hàng sớm nhất có thể.</p></div>",
                "Đang chuẩn bị" => "<div style='background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 10px; border-radius: 5px; margin: 15px 0;'><p><strong>Đơn hàng đang được chuẩn bị!</strong> Chúng tôi sẽ thông báo khi đơn hàng được giao cho đơn vị vận chuyển.</p></div>",
                "Đang giao hàng" => "<div style='background-color: #cce5ff; border: 1px solid #99d6ff; padding: 10px; border-radius: 5px; margin: 15px 0;'><p><strong>Đơn hàng đang trên đường giao đến bạn!</strong> Vui lòng chú ý điện thoại để nhận hàng.</p></div>",
                "Đã giao hàng" => "<div style='background-color: #d4edda; border: 1px solid #c3e6cb; padding: 10px; border-radius: 5px; margin: 15px 0;'><p><strong>Đơn hàng đã được giao thành công!</strong> Cảm ơn bạn đã mua sắm tại cửa hàng chúng tôi.</p></div>",
                _ => ""
            };
        }

        public async Task SendEmployeeCredentialsEmailAsync(string email, string employeeName, string employeeCode, string password)
        {
            try
            {
                var subject = "Thông tin tài khoản nhân viên mới";
                var body = GenerateEmployeeCredentialsEmailBody(email, employeeName, employeeCode, password);

                await SendEmailAsync(email, subject, body);

                _logger.LogInformation($"Đã gửi email thông tin tài khoản cho nhân viên {employeeName} ({employeeCode}) tới {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi gửi email thông tin tài khoản cho nhân viên {employeeName}: {ex.Message}");
                throw new ApplicationException($"Lỗi khi gửi email thông tin tài khoản: {ex.Message}", ex);
            }
        }

        private string GenerateEmployeeCredentialsEmailBody(string email, string employeeName, string employeeCode, string password)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset='UTF-8'></head><body>");
            sb.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>");

            // Header
            sb.AppendLine("<div style='background-color: #28a745; color: white; padding: 20px; text-align: center;'>");
            sb.AppendLine("<h1>Chào mừng bạn đến với đội ngũ nhân viên!</h1>");
            sb.AppendLine("</div>");

            // Content
            sb.AppendLine("<div style='padding: 20px;'>");
            sb.AppendLine($"<p>Xin chào <strong>{employeeName}</strong>,</p>");
            sb.AppendLine("<p>Chúc mừng! Tài khoản nhân viên của bạn đã được tạo thành công. Dưới đây là thông tin đăng nhập của bạn:</p>");

            // Credentials box
            sb.AppendLine("<div style='background-color: #f8f9fa; border: 2px solid #28a745; padding: 20px; border-radius: 10px; margin: 20px 0;'>");
            sb.AppendLine($"<p><strong>Mã nhân viên:</strong> <span style='color: #007bff; font-weight: bold;'>{employeeCode}</span></p>");
            sb.AppendLine($"<p><strong>Email đăng nhập:</strong> <span style='color: #007bff; font-weight: bold;'>{email}</span></p>");
            sb.AppendLine($"<p><strong>Mật khẩu:</strong> <span style='color: #dc3545; font-weight: bold; font-family: monospace;'>{password}</span></p>");
            sb.AppendLine("</div>");

            // Security notice
            sb.AppendLine("<div style='background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 15px 0;'>");
            sb.AppendLine("<p><strong>⚠️ Lưu ý bảo mật:</strong></p>");
            sb.AppendLine("<ul>");
            sb.AppendLine("<li>Vui lòng đổi mật khẩu sau lần đăng nhập đầu tiên</li>");
            sb.AppendLine("<li>Không chia sẻ thông tin đăng nhập với bất kỳ ai</li>");
            sb.AppendLine("<li>Sử dụng mật khẩu mạnh khi thay đổi</li>");
            sb.AppendLine("</ul>");
            sb.AppendLine("</div>");

            sb.AppendLine("<p>Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ với bộ phận IT hoặc quản lý trực tiếp.</p>");
            sb.AppendLine("<p>Chúc bạn làm việc hiệu quả!</p>");
            sb.AppendLine("</div>");

            // Footer
            sb.AppendLine("<div style='background-color: #f8f9fa; padding: 15px; text-align: center; color: #6c757d;'>");
            sb.AppendLine("<p>Đây là email tự động, vui lòng không trả lời email này.</p>");
            sb.AppendLine("<p>Nếu có thắc mắc, vui lòng liên hệ: hr@example.com | 0123-456-789</p>");
            sb.AppendLine("</div>");

            sb.AppendLine("</div></body></html>");

            return sb.ToString();
        }

        public async Task SendDiscountNotificationEmailAsync(PhieuGiamGia phieuGiamGia)
        {
            try
            {
                // Lấy danh sách tất cả khách hàng có email
                var customers = await _context.KhachHang
                    .Where(kh => kh.TrangThai && !string.IsNullOrEmpty(kh.Email))
                    .ToListAsync();

                if (!customers.Any())
                {
                    _logger.LogWarning("Không có khách hàng nào có email để gửi thông báo giảm giá");
                    return;
                }

                var subject = $"🎉 Chương trình giảm giá mới: {phieuGiamGia.TenPhieu}";
                var body = GenerateDiscountNotificationEmailBody(phieuGiamGia);

                // Gửi email cho từng khách hàng
                var emailTasks = customers.Select(async customer =>
                {
                    try
                    {
                        await SendEmailAsync(customer.Email, subject, body);
                        _logger.LogInformation($"Đã gửi email thông báo giảm giá cho khách hàng {customer.TenKhachHang} ({customer.Email})");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Lỗi khi gửi email thông báo giảm giá cho khách hàng {customer.TenKhachHang} ({customer.Email}): {ex.Message}");
                    }
                });

                await Task.WhenAll(emailTasks);

                _logger.LogInformation($"Đã hoàn thành gửi email thông báo giảm giá '{phieuGiamGia.TenPhieu}' cho {customers.Count} khách hàng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi gửi email thông báo giảm giá '{phieuGiamGia.TenPhieu}': {ex.Message}");
                // Không throw exception để không làm gián đoạn việc tạo phiếu giảm giá
            }
        }

        private string GenerateDiscountNotificationEmailBody(PhieuGiamGia phieuGiamGia)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset='UTF-8'></head><body>");
            sb.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>");

            // Header
            sb.AppendLine("<div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>");
            sb.AppendLine("<h1 style='margin: 0; font-size: 28px;'>🎉 Chương trình giảm giá mới!</h1>");
            sb.AppendLine("<p style='margin: 10px 0 0 0; font-size: 16px; opacity: 0.9;'>Cơ hội tuyệt vời dành riêng cho bạn</p>");
            sb.AppendLine("</div>");

            // Content
            sb.AppendLine("<div style='padding: 30px; background-color: #ffffff;'>");
            sb.AppendLine("<p style='font-size: 16px; color: #333; margin-bottom: 20px;'>Xin chào quý khách,</p>");
            sb.AppendLine($"<p style='font-size: 16px; color: #333; line-height: 1.6;'>Chúng tôi vui mừng thông báo về chương trình giảm giá mới <strong style='color: #667eea;'>\"{phieuGiamGia.TenPhieu}\"</strong> đã chính thức có hiệu lực!</p>");

            // Discount details box
            sb.AppendLine("<div style='background: linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%); padding: 25px; border-radius: 15px; margin: 25px 0; text-align: center; border: 3px solid #ff6b6b;'>");
            sb.AppendLine($"<h2 style='color: #d63031; margin: 0 0 15px 0; font-size: 24px;'>MÃ GIẢM GIÁ: <span style='background-color: #d63031; color: white; padding: 8px 15px; border-radius: 25px; font-family: monospace;'>{phieuGiamGia.MaCode}</span></h2>");
            sb.AppendLine($"<p style='font-size: 20px; color: #2d3436; margin: 10px 0; font-weight: bold;'>🔥 GIẢM NGAY {phieuGiamGia.GiaTriGiam}% 🔥</p>");
            
            if (phieuGiamGia.GiaTriGiamToiDa.HasValue)
            {
                sb.AppendLine($"<p style='color: #636e72; margin: 5px 0;'>💰 Giảm tối đa: <strong>{phieuGiamGia.GiaTriGiamToiDa.Value:N0} VNĐ</strong></p>");
            }
            
            if (phieuGiamGia.DonToiThieu.HasValue)
            {
                sb.AppendLine($"<p style='color: #636e72; margin: 5px 0;'>🛒 Đơn hàng tối thiểu: <strong>{phieuGiamGia.DonToiThieu.Value:N0} VNĐ</strong></p>");
            }
            sb.AppendLine("</div>");

            // Validity period
            sb.AppendLine("<div style='background-color: #f8f9fa; border-left: 4px solid #007bff; padding: 20px; margin: 20px 0;'>");
            sb.AppendLine("<h3 style='color: #007bff; margin: 0 0 10px 0; font-size: 18px;'>⏰ Thời gian áp dụng:</h3>");
            sb.AppendLine($"<p style='margin: 5px 0; color: #495057;'><strong>Từ:</strong> {phieuGiamGia.NgayBatDau:dd/MM/yyyy HH:mm}</p>");
            sb.AppendLine($"<p style='margin: 5px 0; color: #495057;'><strong>Đến:</strong> {phieuGiamGia.NgayKetThuc:dd/MM/yyyy HH:mm}</p>");
            sb.AppendLine("</div>");

            // Call to action
            sb.AppendLine("<div style='text-align: center; margin: 30px 0;'>");
            sb.AppendLine("<a href='#' style='display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 15px 30px; text-decoration: none; border-radius: 25px; font-weight: bold; font-size: 16px; box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);'>🛍️ MUA SẮM NGAY</a>");
            sb.AppendLine("</div>");

            // Instructions
            sb.AppendLine("<div style='background-color: #e8f4fd; border: 1px solid #bee5eb; padding: 20px; border-radius: 10px; margin: 20px 0;'>");
            sb.AppendLine("<h3 style='color: #0c5460; margin: 0 0 15px 0; font-size: 16px;'>📋 Cách sử dụng mã giảm giá:</h3>");
            sb.AppendLine("<ol style='color: #0c5460; margin: 0; padding-left: 20px;'>");
            sb.AppendLine("<li>Chọn sản phẩm yêu thích và thêm vào giỏ hàng</li>");
            sb.AppendLine("<li>Tiến hành thanh toán</li>");
            sb.AppendLine($"<li>Nhập mã <strong>{phieuGiamGia.MaCode}</strong> vào ô \"Mã giảm giá\"</li>");
            sb.AppendLine("<li>Nhấn \"Áp dụng\" và hoàn tất đơn hàng</li>");
            sb.AppendLine("</ol>");
            sb.AppendLine("</div>");

            // Terms
            sb.AppendLine("<div style='background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0;'>");
            sb.AppendLine("<p style='margin: 0; color: #856404; font-size: 14px;'><strong>⚠️ Lưu ý:</strong></p>");
            sb.AppendLine("<ul style='color: #856404; font-size: 14px; margin: 10px 0 0 0; padding-left: 20px;'>");
            sb.AppendLine("<li>Mỗi khách hàng chỉ được sử dụng mã này một lần</li>");
            sb.AppendLine("<li>Không áp dụng cùng với các chương trình khuyến mãi khác</li>");
            sb.AppendLine("<li>Mã giảm giá có thể hết hạn sớm nếu đã đạt số lượng tối đa</li>");
            sb.AppendLine("</ul>");
            sb.AppendLine("</div>");

            sb.AppendLine("<p style='color: #333; font-size: 16px; line-height: 1.6;'>Đừng bỏ lỡ cơ hội tuyệt vời này! Hãy nhanh tay mua sắm để tận hưởng ưu đãi hấp dẫn.</p>");
            sb.AppendLine("<p style='color: #333; font-size: 16px;'>Cảm ơn bạn đã tin tưởng và đồng hành cùng chúng tôi! 💝</p>");
            sb.AppendLine("</div>");

            // Footer
            sb.AppendLine("<div style='background-color: #2d3436; color: #ddd; padding: 20px; text-align: center; border-radius: 0 0 10px 10px;'>");
            sb.AppendLine("<p style='margin: 0 0 10px 0; font-size: 14px;'>Đây là email tự động, vui lòng không trả lời email này.</p>");
            sb.AppendLine("<p style='margin: 0; font-size: 14px;'>Nếu có thắc mắc, vui lòng liên hệ: support@example.com | 0123-456-789</p>");
            sb.AppendLine("<div style='margin-top: 15px;'>");
            sb.AppendLine("<a href='#' style='color: #74b9ff; text-decoration: none; margin: 0 10px;'>Website</a>");
            sb.AppendLine("<a href='#' style='color: #74b9ff; text-decoration: none; margin: 0 10px;'>Facebook</a>");
            sb.AppendLine("<a href='#' style='color: #74b9ff; text-decoration: none; margin: 0 10px;'>Instagram</a>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            sb.AppendLine("</div></body></html>");

            return sb.ToString();
        }

        Task IEmailService.SendEmailAsync(string toEmail, string subject, string body)
        {
            return SendEmailAsync(toEmail, subject, body);
        }
    }
}