using BanQuanAu1.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using QuanView.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Cấu hình DbContext
builder.Services.AddDbContext<BanQuanAu1DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2️⃣ Cấu hình HttpClient gọi API
builder.Services.AddHttpClient("MyApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7130/api/");
});
//builder.Services.AddHttpClient("QuanApi", client =>
//{
//    client.BaseAddress = new Uri("https://localhost:7130/"); 
//    client.DefaultRequestHeaders.Accept.Add(
//        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
//});
builder.Services.AddHttpClient("QuanApi", c => c.BaseAddress = new Uri("https://localhost:7130/api/"));

// Đọc cấu hình từ appsettings
var emailConfig = builder.Configuration.GetSection("EmailSettings").Get<EmailConfig>();

// Đăng ký cấu hình và dịch vụ Email
builder.Services.AddSingleton(emailConfig);
builder.Services.AddSingleton<IEmailService, EmailService>();


// 3️⃣ Cấu hình xác thực Google + Cookie
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
    options.CallbackPath = "/signin-google";
    options.Scope.Add("profile");
    options.ClaimActions.MapJsonKey("picture", "picture", "url");

    options.Events = new OAuthEvents
    {
        OnRemoteFailure = context =>
        {
            // 🔁 Redirect về Home/Index kèm error
            context.Response.Redirect("/Home/Index?error=" + Uri.EscapeDataString(context.Failure?.Message ?? "unknown"));
            context.HandleResponse();
            return Task.CompletedTask;
        }
    };
});

// 4️⃣ CORS để gọi từ frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7286")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 5️⃣ Cấu hình dịch vụ MVC
builder.Services.AddControllersWithViews();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// ✅ Thêm HttpContextAccessor để dùng trong controller
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// 6️⃣ Pipeline xử lý request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// 7️⃣ Cấu hình routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=ProductManage}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

