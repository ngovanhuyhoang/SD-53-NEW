using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuanApi.Data;
using QuanApi.Dtos;
using static QuanApi.Controllers.PhieuGiamGiasController;

namespace QuanView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhieuGiamGiaController : Controller
    {
        private readonly HttpClient _http;
        private readonly ILogger<PhieuGiamGiaController> _logger;

        private readonly IEmailService _emailService;

        public PhieuGiamGiaController(IHttpClientFactory factory, ILogger<PhieuGiamGiaController> logger, IEmailService emailService)
        {
            _http = factory.CreateClient("MyApi");
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _http.GetFromJsonAsync<List<PhieuGiamGia>>("PhieuGiamGias")
                           ?? new List<PhieuGiamGia>();
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể tải danh sách phiếu giảm giá.");
                TempData["ErrorMessage"] = "Không thể tải danh sách.";
                return View(new List<PhieuGiamGia>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var phieu = await _http.GetFromJsonAsync<PhieuGiamGia>($"PhieuGiamGias/{id}");
            return phieu == null ? NotFound() : View(phieu);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.KhachHangs = await GetKhachHangsAsync();
            return View(new CreatePhieuGiamGiaDto
            {
                NgayBatDau = DateTime.Today,
                NgayKetThuc = DateTime.Today.AddDays(7),
                NguoiTao = User.Identity?.Name ?? "Admin"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePhieuGiamGiaDto model, bool LaCongKhai, Guid? khachHangId, bool GuiEmail = false)
        {
            // Cập nhật model.LaCongKhai từ parameter
            model.LaCongKhai = LaCongKhai;
            
            // Nếu là công khai hoặc không có khách hàng được chọn, set khachHangId = null
            if (LaCongKhai || khachHangId == Guid.Empty)
                khachHangId = null;

            if (!ModelState.IsValid)
            {
                ViewBag.KhachHangs = await GetKhachHangsAsync();
                return View(model);
            }

            var url = "PhieuGiamGias";
            if (khachHangId.HasValue)
                url += $"?idKhachHang={khachHangId.Value}";

            var response = await _http.PostAsJsonAsync(url, model);
            if (response.IsSuccessStatusCode)
            {
                if (khachHangId.HasValue && GuiEmail)
                {
                    var kh = (await GetKhachHangsAsync()).FirstOrDefault(x => x.IDKhachHang == khachHangId.Value);
                    if (kh != null && !string.IsNullOrEmpty(kh.Email))
                    {
                        await _emailService.SendVoucherEmailAsync(kh.Email, kh.TenKhachHang, model.TenPhieu);
                    }
                }

                var message = LaCongKhai ? "Tạo phiếu giảm giá công khai thành công! Tất cả khách hàng đã được nhận 1 phiếu mỗi người." : "Tạo phiếu giảm giá thành công!";
                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(Index));
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Tạo thất bại: {error}");
            ViewBag.KhachHangs = await GetKhachHangsAsync();
            return View(model);
        }


        // GET: Admin/PhieuGiamGia/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var phieu = await _http.GetFromJsonAsync<PhieuGiamGia>($"PhieuGiamGias/{id}");
            if (phieu == null) return NotFound();

            var khachHangs = await GetKhachHangsAsync();
            ViewBag.KhachHangs = khachHangs;
            Guid? selectedKhachHangId = null;
            var idx = phieu.TenPhieu.IndexOf("(Áp dụng KH");
            if (idx >= 0)
            {
                var code = phieu.TenPhieu.Substring(idx).Trim();
                var maKh = code.Replace("(Áp dụng KH", "").Replace(")", "").Trim();
                var kh = khachHangs.FirstOrDefault(k => k.MaKhachHang == maKh);
                if (kh != null)
                    selectedKhachHangId = kh.IDKhachHang;
            }

            ViewBag.SelectedKhachHangId = selectedKhachHangId;

            return View(phieu);
        }



        // POST: Admin/PhieuGiamGia/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PhieuGiamGia model, Guid? khachHangId, bool LaCongKhai)
        {
            if (id != model.IDPhieuGiamGia) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.KhachHangs = await GetKhachHangsAsync();
                ViewBag.SelectedKhachHangId = khachHangId;
                return View(model);
            }

            model.LaCongKhai = LaCongKhai;
            model.NguoiCapNhat = User.Identity?.Name ?? "Admin";
            model.LanCapNhatCuoi = DateTime.UtcNow;

            var payload = new
            {
                phieu = model,
                khachHangId = LaCongKhai ? null : khachHangId
            };

            var res = await _http.PutAsJsonAsync($"PhieuGiamGias/{id}", payload);
            if (res.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", $"Cập nhật thất bại: {await res.Content.ReadAsStringAsync()}");
            ViewBag.KhachHangs = await GetKhachHangsAsync();
            ViewBag.SelectedKhachHangId = khachHangId;
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var phieu = await _http.GetFromJsonAsync<PhieuGiamGia>($"PhieuGiamGias/{id}");
            return phieu == null ? NotFound() : View(phieu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var res = await _http.DeleteAsync($"PhieuGiamGias/{id}");

            TempData[res.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                res.IsSuccessStatusCode ? "Đã xoá thành công." : "Xoá thất bại.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<KhachHangDto>> GetKhachHangsAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<KhachHangDto>>("KhachHang")
                       ?? new List<KhachHangDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không thể load khách hàng.");
                return new List<KhachHangDto>();
            }
        }
    }
}
