using Microsoft.EntityFrameworkCore;
using QuanLyKTX.Data;
using QuanLyKTX.Models;

namespace QuanLyKTX.Services
{
    public class HoaDonService
    {
        private readonly ApplicationDbContext _context;
        private readonly TinhTienService _tinhTien;

        public HoaDonService(ApplicationDbContext context, TinhTienService tinhTien)
        {
            _context = context;
            _tinhTien = tinhTien;
        }

        public class KetQuaTaoHoaDon
        {
            public int SoHoaDonTaoMoi { get; set; }
            public List<string> Loi { get; set; } = new();
        }

        // Tự động sinh hóa đơn cho TẤT CẢ hợp đồng đang hiệu lực trong 1 tháng/năm
        public async Task<KetQuaTaoHoaDon> TaoHoaDonThangAsync(int thang, int nam)
        {
            var ketQua = new KetQuaTaoHoaDon();

            var hopDongDangHieuLuc = await _context.HopDongs
                .Include(h => h.Phong)
                .Where(h => h.TrangThai == TrangThaiHopDong.DangHieuLuc)
                .ToListAsync();

            foreach (var hd in hopDongDangHieuLuc)
            {
                bool daCoHoaDon = await _context.HoaDons.AnyAsync(h => h.MaHopDong == hd.MaHopDong && h.Thang == thang && h.Nam == nam);
                if (daCoHoaDon) continue;

                var chiSo = await _context.ChiSoDienNuocs
                    .FirstOrDefaultAsync(c => c.MaPhong == hd.MaPhong && c.Thang == thang && c.Nam == nam);

                if (chiSo == null)
                {
                    ketQua.Loi.Add($"Phòng {hd.Phong?.TenPhong}: chưa ghi chỉ số điện nước tháng {thang}/{nam}, bỏ qua.");
                    continue;
                }

                // Đây là chỗ gọi TinhTienService - tính theo bậc thang thay vì đơn giá cố định
                decimal tienDien = await _tinhTien.TinhTienTheoBacAsync(LoaiBacGia.Dien, chiSo.SoDienTieuThu);
                decimal tienNuoc = await _tinhTien.TinhTienTheoBacAsync(LoaiBacGia.Nuoc, chiSo.SoNuocTieuThu);
                decimal tienPhong = hd.Phong?.GiaPhong ?? 0;

                var hoaDon = new HoaDon
                {
                    MaHopDong = hd.MaHopDong,
                    Thang = thang,
                    Nam = nam,
                    TienPhong = tienPhong,
                    TienDien = tienDien,
                    TienNuoc = tienNuoc,
                    TienKhac = 0,
                    TongTien = tienPhong + tienDien + tienNuoc,
                    NgayLap = DateTime.Today,
                    HanThanhToan = DateTime.Today.AddDays(10),
                    TrangThai = TrangThaiHoaDon.ChuaThanhToan
                };

                _context.HoaDons.Add(hoaDon);
                ketQua.SoHoaDonTaoMoi++;
            }

            await _context.SaveChangesAsync();
            return ketQua;
        }

        // Ghi nhận thanh toán, tự cập nhật trạng thái nếu đã trả đủ
        public async Task DanhDauDaThanhToanAsync(int maHoaDon)
        {
            var hoaDon = await _context.HoaDons.FindAsync(maHoaDon);
            if (hoaDon != null)
            {
                hoaDon.TrangThai = TrangThaiHoaDon.DaThanhToan;
                await _context.SaveChangesAsync();
            }
        }
    }
}