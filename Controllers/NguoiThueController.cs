using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKTX.Data;
using QuanLyKTX.Models;

namespace QuanLyKTX.Controllers
{
    public class NguoiThueController : Controller
    {
        private readonly ApplicationDbContext _context;
        public NguoiThueController(ApplicationDbContext context) => _context = context;

        // GET: /NguoiThue - danh sách, có tìm kiếm theo tên/CCCD/SĐT
        public async Task<IActionResult> Index(string? timKiem)
        {
            var query = _context.NguoiThues.AsQueryable();

            if (!string.IsNullOrWhiteSpace(timKiem))
                query = query.Where(n => n.HoTen.Contains(timKiem) || n.CCCD.Contains(timKiem) || n.SDT.Contains(timKiem));

            ViewBag.TimKiem = timKiem;
            var danhSach = await query.OrderBy(n => n.HoTen).ToListAsync();
            return View(danhSach);
        }

        // GET: /NguoiThue/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var nt = await _context.NguoiThues.FirstOrDefaultAsync(n => n.MaNguoiThue == id);
            if (nt == null) return NotFound();
            return View(nt);
        }

        // GET: /NguoiThue/Create
        public IActionResult Create() => View(new NguoiThue());

        // POST: /NguoiThue/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NguoiThue nguoiThue)
        {
            bool trung = await _context.NguoiThues.AnyAsync(n => n.CCCD == nguoiThue.CCCD);
            if (trung)
                ModelState.AddModelError(nameof(nguoiThue.CCCD), "CCCD này đã được đăng ký");

            if (!ModelState.IsValid)
                return View(nguoiThue);

            _context.NguoiThues.Add(nguoiThue);
            await _context.SaveChangesAsync();
            TempData["ThanhCong"] = $"Đã thêm người thuê {nguoiThue.HoTen}";
            return RedirectToAction(nameof(Index));
        }

        // GET: /NguoiThue/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var nt = await _context.NguoiThues.FindAsync(id);
            if (nt == null) return NotFound();
            return View(nt);
        }

        // POST: /NguoiThue/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NguoiThue nguoiThue)
        {
            if (id != nguoiThue.MaNguoiThue) return NotFound();

            bool trung = await _context.NguoiThues.AnyAsync(n => n.CCCD == nguoiThue.CCCD && n.MaNguoiThue != id);
            if (trung)
                ModelState.AddModelError(nameof(nguoiThue.CCCD), "CCCD này đã được đăng ký");

            if (!ModelState.IsValid)
                return View(nguoiThue);

            _context.Update(nguoiThue);
            await _context.SaveChangesAsync();
            TempData["ThanhCong"] = "Đã cập nhật thông tin người thuê";
            return RedirectToAction(nameof(Index));
        }

        // GET: /NguoiThue/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var nt = await _context.NguoiThues.FirstOrDefaultAsync(n => n.MaNguoiThue == id);
            if (nt == null) return NotFound();
            return View(nt);
        }

        // POST: /NguoiThue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nt = await _context.NguoiThues.FindAsync(id);
            if (nt != null)
            {
                _context.NguoiThues.Remove(nt);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Đã xóa người thuê";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}