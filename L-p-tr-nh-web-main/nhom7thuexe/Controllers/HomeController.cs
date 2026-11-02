using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using nhom7thuexe.Helpers;
using nhom7thuexe.Models;
using nhom7thuexe.ViewModels;

namespace nhom7thuexe.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // Trang chủ: "/"
        public async Task<IActionResult> Index()
        {
            var xes = await _context.LayDanhSachXeAsync();
            var nhom = XeCardViewModel.NhomTheoLoai(xes);

            var vm = new HomeIndexViewModel
            {
                LoaiXe = nhom.Select(g => g.LoaiXeMay).ToList(),
                // Mỗi loại lấy tối đa 2 xe đang sẵn sàng, tổng 6 xe nổi bật
                XeNoiBat = XeCardViewModel.NhomTheoLoai(xes.Where(x => x.SanSang))
                    .SelectMany(g => g.Xes.Take(2))
                    .Take(6)
                    .ToList()
            };
            return View(vm);
        }

        [HttpGet("gioi-thieu")]
        public IActionResult GioiThieu()
        {
            return View();
        }

        [HttpGet("lien-he")]
        public IActionResult LienHe()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? code)
        {
            // code = mã HTTP khi đi qua UseStatusCodePagesWithReExecute (vd 404); lỗi hệ thống thì coi là 500
            ViewData["Code"] = code ?? 500;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
