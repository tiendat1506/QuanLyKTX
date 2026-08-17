using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKTX.Data;
using QuanLyKTX.Models;

namespace QuanLyKTX.Controllers
{
    public class PhongController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhongController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Phong - danh sách tất cả phòng, có tìm kiếm + lọc trạng thái
        public async Task<IActionResult> Index(string? timKiem, TrangThaiPhong? trangThai)
        {
            var query = _context.Phongs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(timKiem))
                query = query.Where(p => p.TenPhong.Contains(timKiem) || p.Khu.Contains(timKiem));

            if (trangThai.HasValue)
                query = query.Where(p => p.TrangThai == trangThai);

            ViewBag.TimKiem = timKiem;
            ViewBag.TrangThai = trangThai;

            var danhSach = await query.OrderBy(p => p.Khu).ThenBy(p => p.TenPhong).ToListAsync();
            return View(danhSach);
        }

        // GET: /Phong/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var phong = await _context.Phongs.FirstOrDefaultAsync(p => p.MaPhong == id);
            if (phong == null) return NotFound();
            return View(phong);
        }

        // GET: /Phong/Create
        public IActionResult Create()
        {
            return View(new Phong { NgayDuaVaoSuDung = DateTime.Today });
        }

        // POST: /Phong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Phong phong)
        {
            // Kiểm tra tên phòng trùng trong cùng 1 khu
            bool trung = await _context.Phongs.AnyAsync(p => p.TenPhong == phong.TenPhong && p.Khu == phong.Khu);
            if (trung)
                ModelState.AddModelError(nameof(phong.TenPhong), "Tên phòng đã tồn tại trong khu này");

            if (!ModelState.IsValid)
                return View(phong);

            _context.Phongs.Add(phong);
            await _context.SaveChangesAsync();
            TempData["ThanhCong"] = $"Đã thêm phòng {phong.TenPhong}";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Phong/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null) return NotFound();
            return View(phong);
        }

        // POST: /Phong/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Phong phong)
        {
            if (id != phong.MaPhong) return NotFound();

            bool trung = await _context.Phongs.AnyAsync(p => p.TenPhong == phong.TenPhong && p.Khu == phong.Khu && p.MaPhong != id);
            if (trung)
                ModelState.AddModelError(nameof(phong.TenPhong), "Tên phòng đã tồn tại trong khu này");

            if (!ModelState.IsValid)
                return View(phong);

            try
            {
                _context.Update(phong);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Phongs.AnyAsync(p => p.MaPhong == id))
                    return NotFound();
                throw;
            }

            TempData["ThanhCong"] = $"Đã cập nhật phòng {phong.TenPhong}";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Phong/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var phong = await _context.Phongs.FirstOrDefaultAsync(p => p.MaPhong == id);
            if (phong == null) return NotFound();
            return View(phong);
        }

        // POST: /Phong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong != null)
            {
                _context.Phongs.Remove(phong);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Đã xóa phòng";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
