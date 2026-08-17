using System.ComponentModel.DataAnnotations;

namespace QuanLyKTX.Models
{
    public enum VaiTro
    {
        QuanTri,   // Toàn quyền: quản lý phòng, hợp đồng, hóa đơn, tài khoản
        NhanVien   // Quyền hạn chế hơn, không quản lý tài khoản người dùng khác
    }

    public class NguoiDung
    {
        [Key]
        public int MaNguoiDung { get; set; }

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;

        // Lưu mật khẩu ĐÃ MÃ HÓA bằng BCrypt, không bao giờ lưu chữ thường
        [Required]
        public string MatKhauHash { get; set; } = string.Empty;

        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        public VaiTro VaiTro { get; set; } = VaiTro.NhanVien;

        public bool DangHoatDong { get; set; } = true;
    }
}
