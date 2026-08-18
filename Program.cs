using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using QuanLyKTX.Data;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký DbContext vào hệ thống Dependency Injection,
// dùng connection string "DefaultConnection" đọc từ appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddScoped<QuanLyKTX.Services.TinhTienService>();
    builder.Services.AddScoped<QuanLyKTX.Services.HoaDonService>();

// Đăng ký Cookie Authentication - cơ chế "nhớ đăng nhập" bằng cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";       // chưa đăng nhập -> chuyển hướng tới đây
        options.AccessDeniedPath = "/Account/AccessDenied"; // đăng nhập rồi nhưng không đủ quyền
        options.ExpireTimeSpan = TimeSpan.FromHours(8);     // cookie hết hạn sau 8 giờ
        options.SlidingExpiration = true;                    // còn thao tác thì tự gia hạn
    });

// Add services to the container.
// AuthorizeFilter toàn cục: MẶC ĐỊNH mọi Controller/Action đều yêu cầu đăng nhập,
// trừ khi action/controller đó được đánh dấu [AllowAnonymous] (như trang Login)
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

var app = builder.Build();

// Tự động migrate + seed tài khoản admin mặc định khi chạy lần đầu
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    QuanLyKTX.Data.DbInitializer.SeedTaiKhoanAdmin(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// QUAN TRỌNG: UseAuthentication() phải đứng TRƯỚC UseAuthorization()
// Authentication = "bạn là ai" -> Authorization = "bạn được làm gì"
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
