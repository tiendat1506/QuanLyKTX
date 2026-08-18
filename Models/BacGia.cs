using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKTX.Models
{
    public enum LoaiBacGia
    {
        Dien,
        Nuoc
    }

    // 1 dòng = 1 bậc giá, VD: "từ số 0 đến 50, giá 1984đ/kWh"
    public class BacGia
    {
        [Key]
        public int MaBacGia { get; set; }

        public LoaiBacGia Loai { get; set; }

        // Bậc bắt đầu từ số thứ mấy (VD: 0, 51, 101...)
        public int TuSo { get; set; }

        // Bậc kết thúc ở số thứ mấy - để trống (null) nghĩa là "trở lên", không giới hạn
        public int? DenSo { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal DonGia { get; set; }
    }
}