using Microsoft.AspNetCore.Mvc;
using QuanApi.Data;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using QuanApi.Dtos;

namespace QuanView.Controllers
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }

    public class GioHangController : Controller
    {
        private readonly HttpClient _http;

        public GioHangController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("MyApi");
        }

        // GET: /GioHang/Index
        public async Task<IActionResult> Index(Guid? iduser)
        {
            if (iduser == null || iduser == Guid.Empty)
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<QuanApi.Data.ChiTietGioHang>>("Cart") ?? new List<QuanApi.Data.ChiTietGioHang>();
                foreach (var item in cart)
                {
                    var responseSpct = await _http.GetAsync($"SanPhamChiTiets/{item.IDSanPhamChiTiet}");
                    if (responseSpct.IsSuccessStatusCode)
                    {
                        var spct = await responseSpct.Content.ReadFromJsonAsync<SanPhamChiTietDto>();
                        if (spct != null)
                        {
                            item.GiaBan = spct.price; 
                            item.SanPhamChiTiet = new SanPhamChiTiet
                            {
                                SanPham = new SanPham
                                {
                                    TenSanPham = spct.TenSanPham,
                                    AnhSanPhams = new List<AnhSanPham> {
                                        new AnhSanPham { UrlAnh = spct.AnhDaiDien ?? "/img/default-product.jpg", LaAnhChinh = true }
                                    }
                                },
                                GiaBan = spct.price 
                            };
                        }
                    }
                }
                var gioHang = new QuanApi.Data.GioHang { ChiTietGioHangs = cart };
                return View(gioHang);
            }
            var response = await _http.GetAsync($"GioHangs/getbyuser?iduser={iduser}");
            if (!response.IsSuccessStatusCode)
            {
                ViewData["ErrorMessage"] = "Không thể tải giỏ hàng.";
                return View(new QuanApi.Data.GioHang { ChiTietGioHangs = new List<QuanApi.Data.ChiTietGioHang>() });
            }
            var gioHangDb = await response.Content.ReadFromJsonAsync<QuanApi.Data.GioHang>();
            if (gioHangDb == null)
            {
                gioHangDb = new QuanApi.Data.GioHang { ChiTietGioHangs = new List<QuanApi.Data.ChiTietGioHang>() };
            }
            // Merge giỏ hàng session nếu có
            var cartSession = HttpContext.Session.GetObjectFromJson<List<QuanApi.Data.ChiTietGioHang>>("Cart");
            if (cartSession != null && cartSession.Any())
            {
                foreach (var item in cartSession)
                {
                    await _http.PostAsync($"GioHangs/add?iduser={iduser}&idsp={item.IDSanPhamChiTiet}&soluong={item.SoLuong}", null);
                }
                HttpContext.Session.Remove("Cart");
                var reload = await _http.GetAsync($"GioHangs/getbyuser?iduser={iduser}");
                if (reload.IsSuccessStatusCode)
                    gioHangDb = await reload.Content.ReadFromJsonAsync<QuanApi.Data.GioHang>() ?? gioHangDb;
            }
            return View(gioHangDb);
        }

        // POST: /GioHang/Add
        [HttpPost]
        public async Task<IActionResult> Add(Guid? iduser, Guid idsp, int soluong)
        {
            if (iduser == null || iduser == Guid.Empty)
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<QuanApi.Data.ChiTietGioHang>>("Cart") ?? new List<QuanApi.Data.ChiTietGioHang>();
                var item = cart.FirstOrDefault(x => x.IDSanPhamChiTiet == idsp);
                if (item != null)
                    item.SoLuong += soluong;
                else
                    cart.Add(new QuanApi.Data.ChiTietGioHang {
                        IDChiTietGioHang = Guid.NewGuid(), 
                        IDSanPhamChiTiet = idsp,
                        SoLuong = soluong,
                        GiaBan = 0
                    });
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                TempData["Success"] = "Đã thêm vào giỏ hàng (tạm).";
                return RedirectToAction("Index");
            }
            var response = await _http.PostAsync($"GioHangs/add?iduser={iduser}&idsp={idsp}&soluong={soluong}", null);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Thêm sản phẩm thất bại.";
            }
            return RedirectToAction("Index", new { iduser });
        }

        // POST: /GioHang/Update
        [HttpPost]
        public async Task<IActionResult> Update(Guid idghct, int soluong, Guid? iduser)
        {
            if (iduser == null || iduser == Guid.Empty)
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<QuanApi.Data.ChiTietGioHang>>("Cart") ?? new List<QuanApi.Data.ChiTietGioHang>();
                var item = cart.FirstOrDefault(x => x.IDChiTietGioHang == idghct);
                if (item != null && soluong > 0)
                    item.SoLuong = soluong;
                else if (item != null && soluong <= 0)
                    cart.Remove(item);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                return RedirectToAction("Index");
            }
            var content = JsonContent.Create(soluong);
            var response = await _http.PutAsJsonAsync($"GioHangs/update?idghct={idghct}&soluong={soluong}", content);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Cập nhật thất bại.";
            }
            return RedirectToAction("Index", new { iduser });
        }

        // POST: /GioHang/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(Guid idgiohang, Guid? iduser)
        {
            if (iduser == null || iduser == Guid.Empty)
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<QuanApi.Data.ChiTietGioHang>>("Cart") ?? new List<QuanApi.Data.ChiTietGioHang>();
                var item = cart.FirstOrDefault(x => x.IDChiTietGioHang == idgiohang);
                if (item != null)
                    cart.Remove(item);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                return RedirectToAction("Index");
            }
            var response = await _http.DeleteAsync($"GioHangs/xoa?idgiohang={idgiohang}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Xoá thất bại.";
            }
            return RedirectToAction("Index", new { iduser });
        }
    }
}
