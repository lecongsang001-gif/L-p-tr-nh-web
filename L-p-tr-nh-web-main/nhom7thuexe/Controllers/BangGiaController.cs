using Microsoft.AspNetCore.Mvc;
using nhom7thuexe.Helpers;
using nhom7thuexe.Models;
using nhom7thuexe.ViewModels;

namespace nhom7thuexe.Controllers
{
    public class BangGiaController : Controller
    {
        private readonly AppDbContext _context;

        public BangGiaController(AppDbContext context)
        {
            _context = context;
        }

        // Trang "Bảng giá": giá theo ngày / tuần / tháng + tiền cọc, chia theo loại xe
        [HttpGet("bang-gia")]
        public async Task<IActionResult> Index()
        {
            var xes = await _context.LayDanhSachXeAsync();
            return View(XeCardViewModel.NhomTheoLoai(xes));
        }
    }
}
