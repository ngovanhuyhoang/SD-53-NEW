﻿using Microsoft.AspNetCore.Mvc;
using QuanApi.Data;
using QuanApi.Dtos;
using System.Net.Http;
using System.Net.Http.Json;

namespace QuanView.Controllers
{
    public class SanPhamNguoiDungController : Controller
    {
        private readonly HttpClient _http;

        public SanPhamNguoiDungController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("MyApi");
        }

        // GET: /SanPhamNguoiDung/Index
        public async Task<IActionResult> Index(string search, int? priceFrom, int? priceTo, string sortOrder, int page = 1)
        {
            var pageSize = 9;
            var query = $"SanPhamNguoiDungs?pageNumber={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search))
                query += $"&search={Uri.EscapeDataString(search)}";
            if (priceFrom.HasValue)
                query += $"&priceFrom={priceFrom.Value}";
            if (priceTo.HasValue)
                query += $"&priceTo={priceTo.Value}";

            var response = await _http.GetAsync(query);
            if (!response.IsSuccessStatusCode)
            {
                ViewData["ErrorMessage"] = "Không thể tải danh sách sản phẩm.";
                return View("Error");
            }
            var list = await response.Content.ReadFromJsonAsync<List<SanPhamKhachHangViewModel>>();
            if (!string.IsNullOrEmpty(sortOrder) && list != null)
            {
                if (sortOrder == "asc")
                {
                    list = list.OrderBy(sp => sp.BienThes != null && sp.BienThes.Any() ? sp.BienThes.Min(b => b.GiaSauGiam) : decimal.MaxValue).ToList();
                }
                else if (sortOrder == "desc")
                {
                    list = list.OrderByDescending(sp => sp.BienThes != null && sp.BienThes.Any() ? sp.BienThes.Min(b => b.GiaSauGiam) : decimal.MinValue).ToList();
                }
            }
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = list?.Count ?? 0;
            return View(list);
        }

        // GET: /SanPhamNguoiDung/Detail/{id}
        public async Task<IActionResult> Detail(Guid id)
        {
            var bienTheRes = await _http.GetAsync($"SanPhamNguoiDungs/{id}");
            if (!bienTheRes.IsSuccessStatusCode)
            {
                ViewData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                return View("Error");
            }
            var bienThe = await bienTheRes.Content.ReadFromJsonAsync<SanPhamChiTietDto>();
            if (bienThe == null)
            {
                ViewData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                return View("Error");
            }

            var allBienTheRes = await _http.GetAsync($"SanPhamChiTiets/bysanpham?idsanpham={bienThe.IdSanPham}");
            if (!allBienTheRes.IsSuccessStatusCode)
            {
                ViewData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                return View("Error");
            }
            var allBienThes = await allBienTheRes.Content.ReadFromJsonAsync<List<SanPhamChiTietDto>>();
            if (allBienThes == null || !allBienThes.Any())
            {
                ViewData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                return View("Error");
            }

            var anhRes = await _http.GetAsync($"SanPhams/{bienThe.IdSanPham}");
            string urlAnh = "";
            if (anhRes.IsSuccessStatusCode)
            {
                var sp = await anhRes.Content.ReadFromJsonAsync<SanPhamDto>();
                if (sp != null && sp.DanhSachAnh != null && sp.DanhSachAnh.Any())
                {
                    urlAnh = sp.DanhSachAnh.First().UrlAnh;
                }
            }

            var firstBienThe = allBienThes.FirstOrDefault() ?? bienThe;
            var model = new SanPhamKhachHangViewModel
            {
                TenSanPham = bienThe.TenSanPham,
                DanhMuc = firstBienThe.TenDanhMuc ?? "",
                UrlAnh = urlAnh,
                BienThes = allBienThes.Select(b => new BienTheSanPhamViewModel
                {
                    IDSanPhamChiTiet = b.IdSanPhamChiTiet,
                    Size = b.TenKichCo,
                    Mau = b.TenMauSac,
                    GiaGoc = b.GiaBan,
                    GiaSauGiam = b.price,
                    SoLuong = b.SoLuong
                }).ToList()
            };
            return View(model);
        }
    }
}
