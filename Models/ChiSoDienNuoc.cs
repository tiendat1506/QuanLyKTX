using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKTX.Models
{
    public class ChiSoDienNuoc
    {
        [Key]
        public int MaChiSo { get; set; }

        [Required]
        public int MaPhong { get; set; }

        [Range(1, 12)]
        public int Thang { get; set; }

        [Range(2000, 2100)]
        public int Nam { get; set; }

        public int ChiSoDienCu { get; set; }
        public int ChiSoDienMoi { get; set; }
        public int ChiSoNuocCu { get; set; }
        public int ChiSoNuocMoi { get; set; }

        [DataType(DataType.Date)]
        public DateTime NgayGhi { get; set; } = DateTime.Today;

        // Không lưu vào DB - tự tính từ 2 chỉ số khi cần dùng
        [NotMapped]
        public int SoDienTieuThu => ChiSoDienMoi - ChiSoDienCu;

        [NotMapped]
        public int SoNuocTieuThu => ChiSoNuocMoi - ChiSoNuocCu;

        [ForeignKey("MaPhong")]
        public Phong? Phong { get; set; }
    }
}