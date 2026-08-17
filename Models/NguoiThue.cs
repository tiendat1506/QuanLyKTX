using System.ComponentModel.DataAnnotations;

namespace QuanLyKTX.Models
{
    public class NguoiThue
    {
        [Key]
        public int MaNguoiThue { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập CCCD/CMND")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "CCCD/CMND phải từ 9-12 số")]
        public string CCCD { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        public string? GioiTinh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SDT { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? QueQuan { get; set; }
    }
}