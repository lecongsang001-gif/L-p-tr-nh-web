using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nhom7thuexe.Models;

namespace nhom7thuexe.Controllers
{
    public class XeController : Controller
    {
        private readonly AppDbContext _context;

        public XeController(AppDbContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH XE (Index)
        public async Task<IActionResult> Index()
        {
            var danhSachXe = await _context.Xes.ToListAsync();
            return View(danhSachXe);
        }

        // 2. THÊM MỚI XE (GET)
        public IActionResult Create()
        {
            return View();
        }

        // THÊM MỚI XE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Xe xe)
        {
            if (XeExists(xe.MaXe))
            {
                ModelState.AddModelError("MaXe", "Mã xe này đã tồn tại trong hệ thống!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(xe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(xe);
        }

        // 3. CHỈNH SỬA XE (GET)
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var xe = await _context.Xes.FindAsync(id);
            if (xe == null) return NotFound();

            return View(xe);
        }

        // CHỈNH SỬA XE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Xe xe)
        {
            if (id != xe.MaXe) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(xe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XeExists(xe.MaXe)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(xe);
        }

        // 4. XÓA XE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var xe = await _context.Xes.FindAsync(id);
            if (xe != null)
            {
                _context.Xes.Remove(xe);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool XeExists(string id)
        {
            return _context.Xes.Any(e => e.MaXe == id);
        }
    }
}