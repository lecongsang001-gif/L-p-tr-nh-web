using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using nhom7thuexe.Helpers;
using nhom7thuexe.Models;
using nhom7thuexe.ViewModels;

namespace nhom7thuexe.Controllers
{
    // TODO: khi có đăng nhập thì bật [Authorize(Roles = "QuanLy")] cho cả controller.
    // Hiện chưa có xác thực nên AI CŨNG TRUY CẬP ĐƯỢC /quan-ly/xe -> chỉ dùng khi chạy thử.
    [Route("quan-ly/xe")]
    public class QuanLyXeController : Controller
    {
        private static readonly string[] DuoiAnhChoPhep = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long AnhToiDa = 2 * 1024 * 1024; // 2 MB

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<QuanLyXeController> _logger;

        public QuanLyXeController(AppDbContext context, IWebHostEnvironment env, ILogger<QuanLyXeController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // ===================== 1. DANH SÁCH =====================
        [HttpGet("")]
        public async Task<IActionResult> Index(string? q)
        {
            var xes = await _context.LayDanhSachXeAsync();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var tuKhoa = q.Trim();
                xes = xes.Where(x => Chua(x.MaXe, tuKhoa) || Chua(x.TenXe, tuKhoa)
                                  || Chua(x.TenHang, tuKhoa) || Chua(x.LoaiXeMay, tuKhoa)).ToList();
            }

            ViewData["Q"] = q;
            return View(xes);
        }

        private static bool Chua(string? s, string tuKhoa) =>
            s != null && s.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase);

        // ===================== 2. THÊM =====================
        [HttpGet("them")]
        public IActionResult Them() =>
            View("Form", new XeFormViewModel
            {
                LaThem = true,
                LoaiXeMay = XeFormViewModel.LoaiXeOptions[0],
                TrangThai = "Sẵn sàng"
            });

        [HttpPost("them")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> Them(XeFormViewModel vm)
        {
            vm.LaThem = true;
            vm.MaXe = (vm.MaXe ?? "").Trim();
            KiemTraChung(vm);

            if (vm.MaXe.Length > 0 && await _context.Xes.AnyAsync(x => x.MaXe == vm.MaXe))
                ModelState.AddModelError(nameof(vm.MaXe), "Mã xe này đã tồn tại.");

            if (!ModelState.IsValid) return View("Form", vm);

            string? tenFile = null;
            if (vm.FileAnh is { Length: > 0 })
                tenFile = await LuuAnhAsync(vm.FileAnh);

            var xe = new Xe
            {
                MaXe = vm.MaXe,
                TenXe = vm.TenXe.Trim(),
                TenHang = vm.TenHang.Trim(),
                LoaiXeMay = vm.LoaiXeMay,
                PhanKhoi = vm.PhanKhoi,
                MauSac = string.IsNullOrWhiteSpace(vm.MauSac) ? null : vm.MauSac.Trim(),
                HinhAnh = tenFile,
                TrangThai = vm.TrangThai
            };

            // Bảng giá áp dụng từ hôm nay, chưa có ngày kết thúc
            xe.BangGia.Add(new BangGium
            {
                // MaBangGia = "BG" + DateTime.Now.ToString("yyMMddHHmmssfff"), // <- bỏ comment nếu khóa chính BangGia là chuỗi, không tự tăng
                MaXe = xe.MaXe,
                ApDungTu = DateOnly.FromDateTime(DateTime.Today),
                ApDungDen = null,
                GiaTheoNgay = vm.GiaTheoNgay!.Value,
                GiaTheoTuan = vm.GiaTheoTuan ?? 0,
                GiaTheoThang = vm.GiaTheoThang ?? 0,
                TienCoc = vm.TienCoc ?? 0
            });

            _context.Xes.Add(xe);

            try
            {
                await _context.SaveChangesAsync(); // 1 transaction: xe + giá
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Không thêm được xe {MaXe}", vm.MaXe);
                XoaFileAnh(tenFile);
                ModelState.AddModelError(string.Empty, "Hệ thống chưa lưu được xe. Vui lòng kiểm tra dữ liệu và thử lại.");
                return View("Form", vm);
            }

            TempData["ThongBao"] = $"Đã thêm xe {xe.TenXe} ({xe.MaXe}).";
            return RedirectToAction(nameof(Index));
        }

        // ===================== 3. SỬA =====================
        [HttpGet("sua/{maXe}")]
        public async Task<IActionResult> Sua(string maXe)
        {
            var card = await _context.LayMotXeAsync(maXe); // đã kèm giá đang áp dụng
            if (card == null) return NotFound();

            return View("Form", new XeFormViewModel
            {
                LaThem = false,
                MaXe = card.MaXe,
                TenXe = card.TenXe,
                TenHang = card.TenHang,
                LoaiXeMay = card.LoaiXeMay,
                PhanKhoi = card.PhanKhoi,
                MauSac = card.MauSac,
                HinhAnh = card.HinhAnh,
                TrangThai = string.IsNullOrWhiteSpace(card.TrangThai) ? "Sẵn sàng" : card.TrangThai,
                GiaTheoNgay = card.GiaTheoNgay,
                GiaTheoTuan = card.GiaTheoTuan,
                GiaTheoThang = card.GiaTheoThang,
                TienCoc = card.TienCoc
            });
        }

        [HttpPost("sua/{maXe}")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> Sua(string maXe, XeFormViewModel vm)
        {
            var xe = await _context.Xes.Include(x => x.BangGia).FirstOrDefaultAsync(x => x.MaXe == maXe);
            if (xe == null) return NotFound();

            // Mã xe lấy từ URL, không cho đổi (nó là khóa ngoại của đơn thuê / bảng giá)
            vm.LaThem = false;
            vm.MaXe = maXe;
            vm.HinhAnh = xe.HinhAnh;
            ModelState.Remove(nameof(vm.MaXe));
            KiemTraChung(vm);

            if (!ModelState.IsValid) return View("Form", vm);

            var anhCu = xe.HinhAnh;
            string? anhMoi = null;
            if (vm.FileAnh is { Length: > 0 })
            {
                anhMoi = await LuuAnhAsync(vm.FileAnh);
                xe.HinhAnh = anhMoi;
            }

            xe.TenXe = vm.TenXe.Trim();
            xe.TenHang = vm.TenHang.Trim();
            xe.LoaiXeMay = vm.LoaiXeMay;
            xe.PhanKhoi = vm.PhanKhoi;
            xe.MauSac = string.IsNullOrWhiteSpace(vm.MauSac) ? null : vm.MauSac.Trim();
            xe.TrangThai = vm.TrangThai;

            // Cập nhật đúng dòng giá mà trang khách đang hiển thị (cùng quy tắc với XeCardViewModel.Tu)
            var homNay = DateOnly.FromDateTime(DateTime.Today);
            var gia = xe.BangGia
                          .Where(b => b.ApDungTu <= homNay && (b.ApDungDen == null || b.ApDungDen >= homNay))
                          .OrderByDescending(b => b.ApDungTu)
                          .FirstOrDefault()
                      ?? xe.BangGia.OrderByDescending(b => b.ApDungTu).FirstOrDefault();

            if (gia == null)
            {
                gia = new BangGium
                {
                    // MaBangGia = "BG" + DateTime.Now.ToString("yyMMddHHmmssfff"), // <- nếu khóa chính là chuỗi
                    MaXe = xe.MaXe,
                    ApDungTu = homNay,
                    ApDungDen = null
                };
                xe.BangGia.Add(gia);
            }
            gia.GiaTheoNgay = vm.GiaTheoNgay!.Value;
            gia.GiaTheoTuan = vm.GiaTheoTuan ?? 0;
            gia.GiaTheoThang = vm.GiaTheoThang ?? 0;
            gia.TienCoc = vm.TienCoc ?? 0;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Không sửa được xe {MaXe}", maXe);
                XoaFileAnh(anhMoi);
                ModelState.AddModelError(string.Empty, "Hệ thống chưa lưu được thay đổi. Vui lòng thử lại.");
                return View("Form", vm);
            }

            if (anhMoi != null) await XoaAnhNeuKhongAiDungAsync(anhCu, maXe);

            TempData["ThongBao"] = $"Đã cập nhật xe {xe.TenXe} ({xe.MaXe}).";
            return RedirectToAction(nameof(Index));
        }

        // ===================== 4. XÓA =====================
        [HttpPost("xoa/{maXe}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(string maXe)
        {
            var xe = await _context.Xes.Include(x => x.BangGia).FirstOrDefaultAsync(x => x.MaXe == maXe);
            if (xe == null) return NotFound();

            // Xe đã từng được thuê thì giữ lại để không mất lịch sử đơn / doanh thu
            if (await _context.ChiTietDonThues.AnyAsync(c => c.MaXe == maXe))
            {
                TempData["Loi"] = $"Xe {xe.TenXe} đã có trong đơn thuê nên không thể xóa. " +
                                  "Hãy đổi trạng thái sang \"Bảo trì\" để ẩn khỏi danh sách cho thuê.";
                return RedirectToAction(nameof(Index));
            }

            var anh = xe.HinhAnh;
            _context.RemoveRange(xe.BangGia); // xóa giá trước, rồi mới xóa xe
            _context.Xes.Remove(xe);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Không xóa được xe {MaXe}", maXe);
                TempData["Loi"] = $"Không xóa được xe {xe.TenXe} vì còn dữ liệu liên quan (sự cố, hợp đồng...).";
                return RedirectToAction(nameof(Index));
            }

            await XoaAnhNeuKhongAiDungAsync(anh, maXe);
            TempData["ThongBao"] = $"Đã xóa xe {xe.TenXe} ({maXe}).";
            return RedirectToAction(nameof(Index));
        }

        // ===================== Hàm phụ =====================

        /// <summary>Kiểm tra file ảnh (nếu có) và quan hệ giữa các mức giá.</summary>
        private void KiemTraChung(XeFormViewModel vm)
        {
            if (vm.FileAnh is { Length: > 0 })
            {
                var duoi = Path.GetExtension(vm.FileAnh.FileName).ToLowerInvariant();
                if (!DuoiAnhChoPhep.Contains(duoi))
                    ModelState.AddModelError(nameof(vm.FileAnh), "Chỉ nhận ảnh .jpg, .jpeg, .png, .webp");
                else if (vm.FileAnh.Length > AnhToiDa)
                    ModelState.AddModelError(nameof(vm.FileAnh), "Ảnh tối đa 2 MB");
            }

            if (vm.GiaTheoTuan is > 0 && vm.GiaTheoNgay is > 0 && vm.GiaTheoTuan > vm.GiaTheoNgay * 7)
                ModelState.AddModelError(nameof(vm.GiaTheoTuan), "Giá tuần không nên cao hơn 7 lần giá ngày.");

            if (!XeFormViewModel.LoaiXeOptions.Contains(vm.LoaiXeMay ?? ""))
                ModelState.AddModelError(nameof(vm.LoaiXeMay), "Loại xe không hợp lệ.");

            if (!XeFormViewModel.TrangThaiOptions.Contains(vm.TrangThai ?? ""))
                ModelState.AddModelError(nameof(vm.TrangThai), "Trạng thái không hợp lệ.");
        }

        private string ThuMucAnh => Path.Combine(_env.WebRootPath, "images", "xe");

        /// <summary>Lưu ảnh vào wwwroot/images/xe, trả về tên file (đúng định dạng ViewHelper.HinhXe đang đọc).</summary>
        private async Task<string> LuuAnhAsync(IFormFile file)
        {
            Directory.CreateDirectory(ThuMucAnh);
            var ten = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName).ToLowerInvariant();
            await using var fs = new FileStream(Path.Combine(ThuMucAnh, ten), FileMode.Create);
            await file.CopyToAsync(fs);
            return ten;
        }

        private void XoaFileAnh(string? tenFile)
        {
            if (string.IsNullOrWhiteSpace(tenFile)) return;
            var path = Path.Combine(ThuMucAnh, Path.GetFileName(tenFile));
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        }

        /// <summary>Xóa file ảnh cũ, trừ khi là đường dẫn đầy đủ/URL hoặc xe khác đang dùng chung file đó.</summary>
        private async Task XoaAnhNeuKhongAiDungAsync(string? hinhAnh, string maXe)
        {
            if (string.IsNullOrWhiteSpace(hinhAnh)) return;
            var h = hinhAnh.Trim();
            if (h.StartsWith('/') || h.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return;
            if (await _context.Xes.AnyAsync(x => x.HinhAnh == hinhAnh && x.MaXe != maXe)) return;
            XoaFileAnh(h);
        }
    }
}
