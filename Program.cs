using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;
using WingtipToys.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// Add services to the container.
services.AddControllersWithViews();
services.AddRazorPages();
services.Configure<RazorViewEngineOptions>(options => {
                // /View/Controller/Action.cshtml
                // /MyView/Controller/Action.cshtml
                
                // {0} -> ten Action
                // {1} -> ten Controller
                // {2} -> ten Area
                options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);

                options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");

            });

// Đăng ký dịch vụ Session
services.AddSession(cfg => {                    
cfg.Cookie.Name = "wingtipstoy";                 // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
    cfg.IdleTimeout = new TimeSpan(0,30, 0);    // Thời gian tồn tại của Session
});

services.AddDbContext<ProductContext>(options => 
    {
        options.UseSqlServer(config.GetConnectionString("ProductContext"));
    });

services.AddDefaultIdentity<AppUser>()
        .AddEntityFrameworkStores<ProductContext>()
        .AddDefaultTokenProviders();

services.AddOptions ();                                        // Kích hoạt Options
var mailsettings = config.GetSection ("MailSettings");          // đọc config
services.Configure<MailSettings> (mailsettings);               // đăng ký để Inject
services.AddTransient<IEmailSender, SendMailService>();        // Đăng ký dịch vụ Mail
// Truy cập IdentityOptions
services.Configure<IdentityOptions> (options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true; // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true; // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại

});

// Cấu hình Cookie
services.ConfigureApplicationCookie (options => {
    // options.Cookie.HttpOnly = true;  
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  
    options.LoginPath = $"/login/";                                 // Url đến trang đăng nhập
    options.LogoutPath = $"/logout/";   
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";   // Trang khi User bị cấm truy cập
});
services.Configure<SecurityStampValidatorOptions>(options =>
{
    // Trên 5 giây truy cập lại sẽ nạp lại thông tin User (Role)
    // SecurityStamp trong bảng User đổi -> nạp lại thông tinn Security
    options.ValidationInterval = TimeSpan.FromSeconds(5); 
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
app.MapRazorPages();

app.UseSession();

app.UseRouting();

app.UseAuthentication(); // xac dinh danh tinh 
app.UseAuthorization();  // xac thuc  quyen truy  cap

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
