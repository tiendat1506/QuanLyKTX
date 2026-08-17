using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKTX.Data;
using QuanLyKTX.Models;

namespace QuanLyKTX.Controllers
{
    public class HopDongController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HopDongController(ApplicationDbContext context) => _context = context;

        // GET: /HopDong - danh sách, kèm thông tin Phòng + Người thuê (nhờ Include)
        public async Task<IActionResult> Index(TrangThaiHopDong? trangThai)
        {
            var query = _context.HopDongs
                .Include(h => h.Phong)
                .Include(h => h.NguoiThue)
                .AsQueryable();

            if (trangThai.HasValue)
                query = query.Where(h => h.TrangThai == trangThai);

            ViewBag.TrangThai = trangThai;
            var danhSach = await query.OrderByDescending(h => h.NgayBatDau).ToListAsync();
            return View(danhSach);
        }

        // GET: /HopDong/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var hd = await _context.HopDongs
                .Include(h => h.Phong)
                .Include(h => h.NguoiThue)
                .FirstOrDefaultAsync(h => h.MaHopDong == id);
            if (hd == null) return NotFound();
            return View(hd);
        }

        // GET: /HopDong/Create
        public async Task<IActionResult> Create()
        {
            await NapDanhSachChonAsync();
            return View(new HopDong { NgayBatDau = DateTime.Today });
        }

        // POST: /HopDong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HopDong hopDong)
        {
            var phong = await _context.Phongs.FindAsync(hopDong.MaPhong);

            // Chặn lập hợp đồng cho phòng đã đầy hoặc đang bảo trì
            if (phong != null && phong.TrangThai != TrangThaiPhong.ConTrong)
                ModelState.AddModelError(nameof(hopDong.MaPhong), "Phòng này hiện không còn trống");

            if (!ModelState.IsValid)
            {
                await NapDanhSachChonAsync();
                return View(hopDong);
            }

            _context.HopDongs.Add(hopDong);

            // Tự động cập nhật trạng thái phòng sang "Đã đầy"
            if (phong != null) phong.TrangThai = TrangThaiPhong.DaDay;

            await _context.SaveChangesAsync();
            TempData["ThanhCong"] = "Đã lập hợp đồng mới";
            return RedirectToAction(nameof(Index));
        }

        // GET: /HopDong/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var hd = await _context.HopDongs.FindAsync(id);
            if (hd == null) return NotFound();
            await NapDanhSachChonAsync();
            return View(hd);
        }

        // POST: /HopDong/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HopDong hopDong)
        {
            if (id != hopDong.MaHopDong) return NotFound();

            if (!ModelState.IsValid)
            {
                await NapDanhSachChonAsync();
                return View(hopDong);
            }

            _context.Update(hopDong);
            await _context.SaveChangesAsync();
            TempData["ThanhCong"] = "Đã cập nhật hợp đồng";
            return RedirectToAction(nameof(Index));
        }

        // POST: /HopDong/KetThuc/5 - Kết thúc hợp đồng, trả phòng về trạng thái Trống
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KetThuc(int id)
        {
            var hd = await _context.HopDongs.Include(h => h.Phong).FirstOrDefaultAsync(h => h.MaHopDong == id);
            if (hd == null) return NotFound();

            hd.TrangThai = TrangThaiHopDong.DaKetThuc;
            hd.NgayKetThuc = DateTime.Today;
            if (hd.Phong != null) hd.Phong.TrangThai = TrangThaiPhong.ConTrong;

            await _context.SaveChangesAsync();
            TempData["ThanhCong"] = "Đã kết thúc hợp đồng, phòng được trả về trạng thái Còn trống";
            return RedirectToAction(nameof(Details), new { id });
        }

        // Nạp danh sách Phòng (chỉ phòng còn trống) + Người thuê cho 2 dropdown trong form
        private async Task NapDanhSachChonAsync()
        {
            ViewBag.DanhSachPhong = new SelectList(
                await _context.Phongs.Where(p => p.TrangThai == TrangThaiPhong.ConTrong).OrderBy(p => p.TenPhong).ToListAsync(),
                "MaPhong", "TenPhong");

            ViewBag.DanhSachNguoiThue = new SelectList(
                await _context.NguoiThues.OrderBy(n => n.HoTen).ToListAsync(),
                "MaNguoiThue", "HoTen");
        }
    }
}