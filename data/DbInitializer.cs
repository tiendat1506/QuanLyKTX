using QuanLyKTX.Models;

namespace QuanLyKTX.Data
{
    public static class DbInitializer
    {
        // Tạo sẵn 1 tài khoản admin nếu bảng NguoiDung đang rỗng
        // Tài khoản: admin / Admin@123 (đổi mật khẩu này sau khi đăng nhập lần đầu)
        public static void SeedTaiKhoanAdmin(ApplicationDbContext db)
        {
            if (db.NguoiDungs.Any()) return;

            var admin = new NguoiDung
            {
                TenDangNhap = "admin",
                MatKhauHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                HoTen = "Quản trị viên",
                VaiTro = VaiTro.QuanTri,
                DangHoatDong = true
            };

            db.NguoiDungs.Add(admin);
            db.SaveChanges();
        }
    }
}
