using BanQuanAu1.Web.Data;
using Microsoft.AspNetCore.Mvc;
using QuanView.Models;
using System;
using System.Diagnostics;

namespace QuanView.Controllers
{
    public class HomeController : Controller
    {
        private readonly BanQuanAu1DbContext _context;

        public HomeController(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var banners = _context.Banners.ToList();
            return View(banners);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
