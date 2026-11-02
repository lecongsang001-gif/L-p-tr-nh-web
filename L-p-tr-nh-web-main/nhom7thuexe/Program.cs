using System.Globalization;
using Microsoft.EntityFrameworkCore;
using nhom7thuexe.Models;

var builder = WebApplication.CreateBuilder(args);

// Hiển thị tiền tệ / ngày tháng theo kiểu Việt Nam (80.000đ, 31/10/2026)
var viVN = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = viVN;
CultureInfo.DefaultThreadCurrentUICulture = viVN;

// 1. DbContext
// LƯU Ý: phải xoá phương thức OnConfiguring trong Models/AppDbContext.cs,
// nếu không chuỗi kết nối cứng trong đó sẽ ghi đè chuỗi kết nối ở appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Thông tin thương hiệu (hotline, zalo, địa chỉ...) đọc từ mục "SiteInfo" trong appsettings.json
builder.Services.Configure<SiteInfo>(builder.Configuration.GetSection("SiteInfo"));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Trang chủ của website là Home/Index. Các trang công khai dùng attribute route
// (/thue-xe-may, /bang-gia, /gioi-thieu, /lien-he, /blog, /dang-ky-thue-xe).
// Trang quản lý xe cũ vẫn nằm ở /Xe.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
