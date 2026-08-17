using System.ComponentModel.DataAnnotations;

namespace QuanLyKTX.Models
{
    public enum TrangThaiPhong
    {
        ConTrong,
        DaDay,
        BaoTri
    }

    public class Phong
    {
        [Key]
        public int MaPhong { get; set; }

        public string TenPhong { get; set; } = string.Empty;
        public int Tang { get; set; }
        public string Khu { get; set; } = string.Empty;
        public string LoaiPhong { get; set; } = string.Empty;
        public int SucChua { get; set; }
        public decimal DienTich { get; set; }
        public decimal GiaPhong { get; set; }

        public TrangThaiPhong TrangThai { get; set; }
        public DateTime NgayDuaVaoSuDung { get; set; }
        public string? GhiChu { get; set; }
    }
}
