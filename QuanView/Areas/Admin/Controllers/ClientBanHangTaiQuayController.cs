using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net.Http.Json;
using QuanApi.Dtos;

namespace QuanView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClientBanHangTaiQuayController : Controller
    {
        private readonly HttpClient _httpClient;
        public ClientBanHangTaiQuayController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
        }
        public IActionResult Index()
        {
            return View();
        }

        // Lấy danh sách sản phẩm
        [HttpGet]
        [Route("Admin/ClientBanHangTaiQuay/danh-sach-san-pham")]
        public async Task<IActionResult> GetProducts()
        {
            var response = await _httpClient.GetAsync("BanHangTaiQuay/danh-sach-san-pham");
            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }

        // Lấy danh sách khách hàng
        [HttpGet]
        [Route("Admin/ClientBanHangTaiQuay/danh-sach-khach-hang")]
        public async Task<IActionResult> GetCustomers()
        {
            var response = await _httpClient.GetAsync("BanHangTaiQuay/danh-sach-khach-hang");
            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }

        // Tìm kiếm khách hàng
        [HttpGet]
        [Route("Admin/ClientBanHangTaiQuay/tim-kiem-khach-hang")]
        public async Task<IActionResult> SearchCustomer(string query)
        {
            var response = await _httpClient.GetAsync($"BanHangTaiQuay/tim-kiem-khach-hang?query={Uri.EscapeDataString(query)}");
            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }

        // Thêm khách hàng mới
        [HttpPost]
        [Route("Admin/ClientBanHangTaiQuay/them-khach-hang")]
        public async Task<IActionResult> AddCustomer([FromBody] TaoKhachHangDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("BanHangTaiQuay/them-khach-hang", dto);
            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }

        // Kiểm tra mã giảm giá
        [HttpGet]
        [Route("Admin/ClientBanHangTaiQuay/kiem-tra-ma-giam-gia")]
        public async Task<IActionResult> CheckDiscount(string code)
        {
            var response = await _httpClient.GetAsync($"BanHangTaiQuay/kiem-tra-ma-giam-gia?code={Uri.EscapeDataString(code)}");
            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }

        // Thanh toán hóa đơn
        [HttpPost]
        [Route("Admin/ClientBanHangTaiQuay/thanh-toan")]
        public async Task<IActionResult> PayInvoice([FromBody] object dto)
        {
            var response = await _httpClient.PostAsJsonAsync("BanHangTaiQuay/thanh-toan", dto);
            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }

        // Lấy danh sách phương thức thanh toán
        [HttpGet]
        [Route("Admin/ClientBanHangTaiQuay/danh-sach-phuong-thuc-thanh-toan")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var response = await _httpClient.GetAsync("BanHangTaiQuay/danh-sach-phuong-thuc-thanh-toan");
            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }
    }
}