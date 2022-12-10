using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;
using WingtipToys.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// Add services to the container.
services.AddControllersWithViews();
services.AddRazorPages();

// Đăng ký dịch vụ Session
services.AddSession(cfg => {                    
cfg.Cookie.Name = "wingtipstoy";                 // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
    cfg.IdleTimeout = new TimeSpan(0,30, 0);    // Thời gian tồn tại của Session
});

services.AddDbContext<ProductContext>(options => 
    {
        options.UseSqlServer(config.GetConnectionString("ProductContext"));
    });
services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
services.AddTransient<CartService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
