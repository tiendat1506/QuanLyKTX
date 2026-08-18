using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKTX.Models
{
    public enum TrangThaiHoaDon
    {
        ChuaThanhToan,
        DaThanhToan,
        QuaHan
    }

    public class HoaDon
    {
        [Key]
        public int MaHoaDon { get; set; }

        [Required]
        public int MaHopDong { get; set; }

        [Range(1, 12)]
        public int Thang { get; set; }

        [Range(2000, 2100)]
        public int Nam { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal TienPhong { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal TienDien { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal TienNuoc { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal TienKhac { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal TongTien { get; set; }

        [DataType(DataType.Date)]
        public DateTime NgayLap { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime HanThanhToan { get; set; } = DateTime.Today.AddDays(10);

        public TrangThaiHoaDon TrangThai { get; set; } = TrangThaiHoaDon.ChuaThanhToan;

        [ForeignKey("MaHopDong")]
        public HopDong? HopDong { get; set; }
    }
}