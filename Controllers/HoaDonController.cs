using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKTX.Data;
using QuanLyKTX.Models;
using QuanLyKTX.Services;

namespace QuanLyKTX.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HoaDonService _hoaDonService;

        public HoaDonController(ApplicationDbContext context, HoaDonService hoaDonService)
        {
            _context = context;
            _hoaDonService = hoaDonService;
        }

        // GET: /HoaDon - danh sách, lọc theo tháng/năm/trạng thái
        public async Task<IActionResult> Index(int? thang, int? nam, TrangThaiHoaDon? trangThai)
        {
            var query = _context.HoaDons
                .Include(h => h.HopDong).ThenInclude(hd => hd!.Phong)
                .Include(h => h.HopDong).ThenInclude(hd => hd!.NguoiThue)
                .AsQueryable();

            if (thang.HasValue) query = query.Where(h => h.Thang == thang);
            if (nam.HasValue) query = query.Where(h => h.Nam == nam);
            if (trangThai.HasValue) query = query.Where(h => h.TrangThai == trangThai);

            ViewBag.Thang = thang;
            ViewBag.Nam = nam;
            ViewBag.TrangThai = trangThai;

            var danhSach = await query.OrderByDescending(h => h.Nam).ThenByDescending(h => h.Thang).ToListAsync();
            return View(danhSach);
        }

        // GET: /HoaDon/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.HopDong).ThenInclude(hd => hd!.Phong)
                .Include(h => h.HopDong).ThenInclude(hd => hd!.NguoiThue)
                .FirstOrDefaultAsync(h => h.MaHoaDon == id);
            if (hoaDon == null) return NotFound();
            return View(hoaDon);
        }

        // GET: /HoaDon/TaoHangLoat - form chọn tháng/năm để sinh hóa đơn tự động
        public IActionResult TaoHangLoat()
        {
            return View();
        }

        // POST: /HoaDon/TaoHangLoat - gọi Service tính tiền theo bậc thang đã làm ở bước trước
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoHangLoat(int thang, int nam)
        {
            var ketQua = await _hoaDonService.TaoHoaDonThangAsync(thang, nam);

            if (ketQua.SoHoaDonTaoMoi > 0)
                TempData["ThanhCong"] = $"Đã tạo {ketQua.SoHoaDonTaoMoi} hóa đơn cho tháng {thang}/{nam}";

            if (ketQua.Loi.Any())
                TempData["Loi"] = string.Join(" | ", ketQua.Loi);

            return RedirectToAction(nameof(Index));
        }

        // POST: /HoaDon/DanhDauDaThanhToan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhDauDaThanhToan(int id)
        {
            await _hoaDonService.DanhDauDaThanhToanAsync(id);
            TempData["ThanhCong"] = "Đã đánh dấu hóa đơn là đã thanh toán";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}