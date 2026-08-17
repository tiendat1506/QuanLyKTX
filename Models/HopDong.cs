using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKTX.Models
{
    public enum TrangThaiHopDong
    {
        DangHieuLuc,   // Đang hiệu lực
        DaKetThuc,     // Đã kết thúc
        DaHuy          // Đã hủy
    }

    public class HopDong
    {
        [Key]
        public int MaHopDong { get; set; }

        [Required]
        public int MaPhong { get; set; }

        [Required]
        public int MaNguoiThue { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgayBatDau { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime? NgayKetThuc { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TienCoc { get; set; }

        public TrangThaiHopDong TrangThai { get; set; } = TrangThaiHopDong.DangHieuLuc;

        [StringLength(500)]
        public string? GhiChu { get; set; }

        // Navigation properties - để truy cập thông tin Phòng/Người thuê liên kết
        // mà không cần join thủ công
        [ForeignKey("MaPhong")]
        public Phong? Phong { get; set; }

        [ForeignKey("MaNguoiThue")]
        public NguoiThue? NguoiThue { get; set; }
    }
}