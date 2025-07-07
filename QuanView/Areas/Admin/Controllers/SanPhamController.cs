using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using QuanView.Areas.Admin.Models;

[Area("Admin")]
public class SanPhamController : Controller
{
    private readonly HttpClient _http;

    public SanPhamController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("MyApi");
    }

    public async Task<IActionResult> Index()
    {
        var response = await _http.GetAsync("sanphams");
        if (!response.IsSuccessStatusCode) return View("Error");

        var json = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<SanPhamDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        foreach (var sp in products)
        {
            // ✅ Đảm bảo gọi đúng endpoint GET api/sanphamchitiets?idsanpham=...
            var res = await _http.GetAsync($"sanphamchitiets/bysanpham?idsanpham={sp.IDSanPham}");

            if (res.IsSuccessStatusCode)
            {
                var ctJson = await res.Content.ReadAsStringAsync();
                sp.ChiTietSanPhams = JsonSerializer.Deserialize<List<SanPhamChiTietDto>>(ctJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }

        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        await LoadDropdownData();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SanPhamDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownData();
            return View(dto);
        }

        dto.IDSanPham = Guid.NewGuid();

        var response = await _http.PostAsJsonAsync("sanphams", dto);
        if (!response.IsSuccessStatusCode)
        {
            var msg = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Lỗi API: {response.StatusCode} - {msg}");
            await LoadDropdownData();
            return View(dto);
        }

        if (dto.ChiTietSanPhams != null && dto.ChiTietSanPhams.Any())
        {
            foreach (var ct in dto.ChiTietSanPhams)
            {
                ct.IdSanPhamChiTiet = Guid.NewGuid();
                ct.IdSanPham = dto.IDSanPham;

                var res = await _http.PostAsJsonAsync("sanphamchitiets", ct);
                if (!res.IsSuccessStatusCode)
                {
                    var msg = await res.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Lỗi lưu biến thể: {msg}");

                    // Nạp lại dữ liệu dropdown nếu có lỗi
                    await LoadDropdownData();
                    return View(dto);
                }

            }
        }


        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await _http.GetAsync($"sanphams/{id}");
        if (!response.IsSuccessStatusCode) return NotFound();

        var dto = await response.Content.ReadFromJsonAsync<SanPhamDto>();

        var res = await _http.GetAsync($"sanphamchitiets/bysanpham?idsanpham={id}");
        if (res.IsSuccessStatusCode)
        {
            var ctJson = await res.Content.ReadAsStringAsync();
            dto.ChiTietSanPhams = JsonSerializer.Deserialize<List<SanPhamChiTietDto>>(ctJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        await LoadDropdownData();
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, SanPhamDto dto)
    {
        var response = await _http.PutAsJsonAsync($"sanphams/{id}", dto);
        if (!response.IsSuccessStatusCode)
        {
            var msg = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Lỗi API: {response.StatusCode} - {msg}");
            await LoadDropdownData();
            return View(dto);
        }

        if (dto.ChiTietSanPhams != null)
        {
            foreach (var ct in dto.ChiTietSanPhams)
            {
                var res = await _http.PutAsJsonAsync($"sanphamchitiets/{ct.IdSanPhamChiTiet}", ct);
                if (!res.IsSuccessStatusCode)
                {
                    var msg = await res.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Lỗi cập nhật biến thể: {msg}");
                }
            }
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _http.GetAsync($"sanphams/{id}");
        if (!response.IsSuccessStatusCode) return NotFound();

        var dto = await response.Content.ReadFromJsonAsync<SanPhamDto>();

        var ctRes = await _http.GetAsync($"sanphamchitiets/bysanpham?idsanpham={id}");
        if (ctRes.IsSuccessStatusCode)
        {
            var ctJson = await ctRes.Content.ReadAsStringAsync();
            dto.ChiTietSanPhams = JsonSerializer.Deserialize<List<SanPhamChiTietDto>>(ctJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        return View(dto);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _http.DeleteAsync($"sanphams/{id}");
        return RedirectToAction("Index");
    }

    private async Task LoadDropdownData()
    {
        ViewBag.ChatLieus = await GetSelectList("chatlieu", "idChatLieu", "tenChatLieu");
        ViewBag.DanhMucs = await GetSelectList("danhmucs", "idDanhMuc", "tenDanhMuc");
        ViewBag.ThuongHieus = await GetSelectList("thuonghieu", "idThuongHieu", "tenThuongHieu");
        ViewBag.LoaiOngs = await GetSelectList("loaiong", "idLoaiOng", "tenLoaiOng");
        ViewBag.KieuDangs = await GetSelectList("kieudang", "idKieuDang", "tenKieuDang");
        ViewBag.LungQuans = await GetSelectList("lungquan", "idLungQuan", "tenLungQuan");
        ViewBag.KichCos = await GetSelectList("kichcos", "idKichCo", "tenKichCo");
        ViewBag.MauSacs = await GetSelectList("mausacs", "idMauSac", "tenMauSac");
        ViewBag.HoaTiets = await GetSelectList("hoatiets", "idHoaTiet", "tenHoaTiet");
    }

    private async Task<List<SelectListItem>> GetSelectList(string url, string idField, string nameField)
    {
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return new List<SelectListItem>();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement.EnumerateArray()
            .Select(e => new SelectListItem
            {
                Value = e.GetProperty(idField).ToString(),
                Text = e.GetProperty(nameField).GetString()
            }).ToList();
    }
}
