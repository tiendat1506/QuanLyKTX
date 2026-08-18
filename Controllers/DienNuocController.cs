using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKTX.Data;
using QuanLyKTX.Models;

namespace QuanLyKTX.Controllers
{
    public class DienNuocController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DienNuocController(ApplicationDbContext context) => _context = context;

        // GET: /DienNuoc - danh sách, lọc theo tháng/năm
        public async Task<IActionResult> Index(int? thang, int? nam)
        {
            var query = _context.ChiSoDienNuocs.Include(c => c.Phong).AsQueryable();

            if (thang.HasValue) query = query.Where(c => c.Thang == thang);
            if (nam.HasValue) query = query.Where(c => c.Nam == nam);

            ViewBag.Thang = thang;
            ViewBag.Nam = nam;

            var danhSach = await query
                .OrderByDescending(c => c.Nam).ThenByDescending(c => c.Thang).ThenBy(c => c.Phong!.TenPhong)
                .ToListAsync();
            return View(danhSach);
        }

        // GET: /DienNuoc/Create
        public async Task<IActionResult> Create()
        {
            await NapDanhSachPhongAsync();
            return View(new ChiSoDienNuoc { Thang = DateTime.Today.Month, Nam = DateTime.Today.Year });
        }

        // POST: /DienNuoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChiSoDienNuoc chiSo)
        {
            // Chỉ số mới phải >= chỉ số cũ (đồng hồ không chạy lùi)
            if (chiSo.ChiSoDienMoi < chiSo.ChiSoDienCu)
                ModelState.AddModelError(nameof(chiSo.ChiSoDienMoi), "Chỉ số điện mới phải lớn hơn hoặc bằng chỉ số cũ");
            if (chiSo.ChiSoNuocMoi < chiSo.ChiSoNuocCu)
                ModelState.AddModelError(nameof(chiSo.ChiSoNuocMoi), "Chỉ số nước mới phải lớn hơn hoặc bằng chỉ số cũ");

            bool daCo = await _context.ChiSoDienNuocs.AnyAsync(c => c.MaPhong == chiSo.MaPhong && c.Thang == chiSo.Thang && c.Nam == chiSo.Nam);
            if (daCo)
                ModelState.AddModelError(string.Empty, "Phòng này đã có chỉ số của tháng/năm được chọn");

            if (!ModelState.IsValid)
            {
                await NapDanhSachPhongAsync();
                return View(chiSo);
            }

            _context.ChiSoDienNuocs.Add(chiSo);
            await _context.SaveChangesAsync();
            TempData["ThanhCong"] = "Đã ghi chỉ số điện nước";
            return RedirectToAction(nameof(Index));
        }

        // GET: /DienNuoc/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var chiSo = await _context.ChiSoDienNuocs.FindAsync(id);
            if (chiSo == null) return NotFound();
            await NapDanhSachPhongAsync();
            return View(chiSo);
        }

        // POST: /DienNuoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChiSoDienNuoc chiSo)
        {
            if (id != chiSo.MaChiSo) return NotFound();

            if (chiSo.ChiSoDienMoi < chiSo.ChiSoDienCu)
                ModelState.AddModelError(nameof(chiSo.ChiSoDienMoi), "Chỉ số điện mới phải lớn hơn hoặc bằng chỉ số cũ");
            if (chiSo.ChiSoNuocMoi < chiSo.ChiSoNuocCu)
                ModelState.AddModelError(nameof(chiSo.ChiSoNuocMoi), "Chỉ số nước mới phải lớn hơn hoặc bằng chỉ số cũ");

            if (!ModelState.IsValid)
            {
                await NapDanhSachPhongAsync();
                return View(chiSo);
            }

            _context.Update(chiSo);
            await _context.SaveChangesAsync();
            TempData["ThanhCong"] = "Đã cập nhật chỉ số";
            return RedirectToAction(nameof(Index));
        }

        // GET: /DienNuoc/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var chiSo = await _context.ChiSoDienNuocs.Include(c => c.Phong).FirstOrDefaultAsync(c => c.MaChiSo == id);
            if (chiSo == null) return NotFound();
            return View(chiSo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chiSo = await _context.ChiSoDienNuocs.FindAsync(id);
            if (chiSo != null)
            {
                _context.ChiSoDienNuocs.Remove(chiSo);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Đã xóa bản ghi chỉ số";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task NapDanhSachPhongAsync()
        {
            ViewBag.DanhSachPhong = new SelectList(
                await _context.Phongs.OrderBy(p => p.TenPhong).ToListAsync(),
                "MaPhong", "TenPhong");
        }
    }
}