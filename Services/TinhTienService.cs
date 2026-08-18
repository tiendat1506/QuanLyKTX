using Microsoft.EntityFrameworkCore;
using QuanLyKTX.Data;
using QuanLyKTX.Models;

namespace QuanLyKTX.Services
{
    // Toàn bộ logic "tính tiền theo bậc thang" tách riêng khỏi Controller,
    // để dễ test độc lập và tái sử dụng ở nhiều nơi (hóa đơn, xem trước giá...)
    public class TinhTienService
    {
        private readonly ApplicationDbContext _context;
        public TinhTienService(ApplicationDbContext context) => _context = context;

        // Tính tiền điện/nước theo bậc thang cho 1 lượng tiêu thụ cụ thể
        // VD: dùng 120 kWh, bậc 1 (0-50) giá 1800đ, bậc 2 (51-100) giá 2000đ, bậc 3 (100+) giá 2500đ
        // => 50*1800 + 50*2000 + 20*2500 (không phải lấy nguyên 120 * giá bậc cao nhất)
        public async Task<decimal> TinhTienTheoBacAsync(LoaiBacGia loai, int soLuongTieuThu)
        {
            if (soLuongTieuThu <= 0) return 0;

            var cacBac = await _context.BacGias
                .Where(b => b.Loai == loai)
                .OrderBy(b => b.TuSo)
                .ToListAsync();

            decimal tongTien = 0;
            int soConLai = soLuongTieuThu;

            foreach (var bac in cacBac)
            {
                if (soConLai <= 0) break;

                // Số lượng thuộc về bậc này: nếu DenSo null (bậc cuối, không giới hạn) thì lấy hết phần còn lại
                int soLuongBacNay = bac.DenSo.HasValue
                    ? Math.Min(soConLai, bac.DenSo.Value - bac.TuSo)
                    : soConLai;

                tongTien += soLuongBacNay * bac.DonGia;
                soConLai -= soLuongBacNay;
            }

            return tongTien;
        }
    }
}