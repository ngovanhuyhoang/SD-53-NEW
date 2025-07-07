﻿using Microsoft.AspNetCore.Mvc;
using QuanApi.Data;
using QuanView.ViewModels;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace QuanView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LungQuanController : Controller
    {
        private readonly HttpClient _httpClient;

        public LungQuanController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("MyApi");
        }

        public async Task<IActionResult> Index(string? keyword, int page = 1, int pageSize = 10)
        {
            // Gọi API GetPaged kèm keyword nếu có
            var url = $"LungQuan/paged?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(keyword))
                url += $"&keyword={Uri.EscapeDataString(keyword)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Không thể tải danh sách lưng quần.";
                return View(new List<LungQuan>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var result = JsonSerializer.Deserialize<PagedResult<LungQuan>>(json, options);

            // Gửi các biến ra view để làm phân trang
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = result.Total;
            ViewBag.Keyword = keyword ?? "";

            return View(result.Data);
        }

        public async Task<IActionResult> Create()
        {
            return PartialView("_CreatePartial", new LungQuan());
        }

        [HttpPost]
        public async Task<IActionResult> Create(LungQuan model)
        {
            Console.WriteLine("📥 MVC nhận Create từ View:");
            Console.WriteLine($"MaLungQuan: {model.MaLungQuan}, TenLungQuan: {model.TenLungQuan}, TrangThai: {model.TrangThai}");

            model.NgayTao = DateTime.Now;
            model.NguoiTao = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("LungQuan/Create", content);

            Console.WriteLine($"📤 Gửi API xong, Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Lỗi từ API: {error}");
                return Json(new { success = false, message = "Không thêm được lưng quần" });
            }

            TempData["message"] = "Tạo lưng quần thành công";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var lq = await _httpClient.GetFromJsonAsync<LungQuan>($"LungQuan/{id}");
            return PartialView("_EditPartial", lq);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LungQuan model)
        {
            model.LanCapNhatCuoi = DateTime.Now;
            model.NguoiCapNhat = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"LungQuan/{model.IDLungQuan}", content);

            return Json(new { success = response.IsSuccessStatusCode });
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var lq = await _httpClient.GetFromJsonAsync<LungQuan>($"LungQuan/{id}");
            return PartialView("_DetailsPartial", lq);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"LungQuan/{id}");
            TempData["message"] = "Xóa lưng quần thành công";
            return Json(new { success = response.IsSuccessStatusCode });
        }

        public class ToggleStatusRequest
        {
            public Guid Id { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus([FromBody] ToggleStatusRequest req)
        {
            Console.WriteLine($"📥 MVC nhận ToggleStatus với ID: {req.Id}");
            var response = await _httpClient.PutAsync($"LungQuan/ToggleStatus/{req.Id}", null);
            Console.WriteLine($"📤 API trả về status: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"📩 Nội dung trả về: {json}");

            if (!response.IsSuccessStatusCode)
                return Json(new { success = false });

            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return Json(new { success = true, trangThai = data["trangThai"] });
        }
    }
}
