using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKTX.Data;
using QuanLyKTX.Models;

namespace QuanLyKTX.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context) => _context = context;

        // GET: /Account/Login
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string tenDangNhap, string matKhau, bool ghiNhoDangNhap, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu");
                return View();
            }

            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(n => n.TenDangNhap == tenDangNhap);

            // So sánh mật khẩu: BCrypt tự so sánh hash, không bao giờ so sánh chuỗi thường trực tiếp
            bool hopLe = nguoiDung != null && BCrypt.Net.BCrypt.Verify(matKhau, nguoiDung.MatKhauHash);

            if (!hopLe || nguoiDung == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng");
                return View();
            }

            if (!nguoiDung.DangHoatDong)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa");
                return View();
            }

            // Claims = "các mẩu thông tin mô tả người dùng", đóng gói vào cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, nguoiDung.MaNguoiDung.ToString()),
                new Claim(ClaimTypes.Name, nguoiDung.TenDangNhap),
                new Claim("HoTen", nguoiDung.HoTen),
                new Claim(ClaimTypes.Role, nguoiDung.VaiTro.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = ghiNhoDangNhap, // true = cookie tồn tại cả khi đóng trình duyệt
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => View();
    }
}
