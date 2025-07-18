using Microsoft.AspNetCore.Mvc;
using QuanApi.Data;
using System.Net.Http;
using System.Net.Http.Json;

namespace QuanView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BanHangTaiQuayController : Controller
    {
        private readonly HttpClient _httpClient;

        public BanHangTaiQuayController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
        }

        
    }
}
