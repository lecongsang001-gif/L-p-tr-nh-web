using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nhom7thuexe.Helpers;
using nhom7thuexe.Models;
using nhom7thuexe.ViewModels;

namespace nhom7thuexe.Controllers
{
    public class ThueXeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ThueXeController> _logger;

        public ThueXeController(AppDbContext context, ILogger<ThueXeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 1. Trang "Thuê Xe Máy": danh sách xe chia theo loại. ?loai=Xe tay ga để lọc 1 loại.
        [HttpGet("thue-xe-may")]
        public async Task<IActionResult> Index(string? loai)
        {
            var xes = await _context.LayDanhSachXeAsync();

            // Danh sách tất cả loại xe (trước khi lọc) để vẽ thanh lọc "Tất cả | Xe tay ga | Xe số ..."
            ViewData["TatCaLoai"] = XeCardViewModel.NhomTheoLoai(xes).Select(g => g.LoaiXeMay).ToList();

            if (!string.IsNullOrWhiteSpace(loai))
            {
                xes = xes.Where(x => string.Equals(x.LoaiXeMay, loai.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewData["Loai"] = loai;
            return View(XeCardViewModel.NhomTheoLoai(xes));
        }

        // 2. Form "ĐĂNG KÝ NGAY" (GET): /dang-ky-thue-xe?maXe=X001
        [HttpGet("dang-ky-thue-xe")]
        public async Task<IActionResult> DangKy(string? maXe)
        {
            if (string.IsNullOrWhiteSpace(maXe)) return RedirectToAction(nameof(Index));

            var xe = await _context.LayMotXeAsync(maXe);
            if (xe == null) return NotFound();

            return View(new DangKyThueXeViewModel { MaXe = xe.MaXe, Xe = xe });
        }

        // 3. Gửi đăng ký (POST): tạo KhachHang (nếu mới) + HopDong + DonThueXe + ChiTietDonThue
        [HttpPost("dang-ky-thue-xe")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyThueXeViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.MaXe)) return RedirectToAction(nameof(Index));

            var xe = await _context.LayMotXeAsync(vm.MaXe);
            if (xe == null) return NotFound();
            vm.Xe = xe;

            // --- Kiểm tra dữ liệu nhập ---
            if (!xe.SanSang)
                ModelState.AddModelError(string.Empty, "Xe này hiện chưa sẵn sàng cho thuê. Vui lòng chọn xe khác hoặc gọi hotline.");
            if (vm.NgayNhan.Date < DateTime.Today)
                ModelState.AddModelError(nameof(vm.NgayNhan), "Ngày nhận xe không được ở trong quá khứ.");
            if (vm.NgayTra.Date <= vm.NgayNhan.Date)
                ModelState.AddModelError(nameof(vm.NgayTra), "Ngày trả xe phải sau ngày nhận xe.");

            if (!ModelState.IsValid) return View(vm);

            // --- Kiểm tra khách hàng (Sdt, Cccd, Email đều có unique index) ---
            var sdt = vm.Sdt.Trim();
            var cccd = vm.Cccd.Trim();
            var email = vm.Email.Trim();

            var khach = await _context.KhachHangs.FirstOrDefaultAsync(k => k.Sdt == sdt);
            if (khach != null)
            {
                if (khach.Cccd.Trim() != cccd)
                    ModelState.AddModelError(nameof(vm.Cccd), "Số điện thoại này đã đăng ký với CCCD khác. Vui lòng kiểm tra lại hoặc gọi hotline.");
            }
            else
            {
                if (await _context.KhachHangs.AnyAsync(k => k.Cccd == cccd))
                    ModelState.AddModelError(nameof(vm.Cccd), "Số CCCD này đã được đăng ký với số điện thoại khác.");
                if (await _context.KhachHangs.AnyAsync(k => k.Email == email))
                    ModelState.AddModelError(nameof(vm.Email), "Email này đã được sử dụng.");
            }

            if (!ModelState.IsValid) return View(vm);

            // --- Tạo dữ liệu ---
            var stamp = DateTime.Now.ToString("yyMMddHHmmssfff");
            var maDon = "DT" + stamp;

            if (khach == null)
            {
                khach = new KhachHang
                {
                    MaKhachHang = "KH" + stamp,
                    HoTen = vm.HoTen.Trim(),
                    Sdt = sdt,
                    Email = email,
                    Cccd = cccd,
                    Gplx = vm.CoGplx,
                    DiaChi = string.IsNullOrWhiteSpace(vm.DiaChi) ? null : vm.DiaChi.Trim()
                };
                _context.KhachHangs.Add(khach);
            }

            var soNgay = (vm.NgayTra.Date - vm.NgayNhan.Date).Days;
            var giaNgay = xe.GiaTheoNgay ?? 0m;
            var tongTien = giaNgay * soNgay; // tạm tính theo giá ngày, nhân viên sẽ xác nhận lại

            // DonThueXe.MaDon là khóa ngoại tới HopDong.MaDon => phải có HopDong cùng mã đơn
            var hopDong = new HopDong
            {
                MaHopDong = "HD" + stamp,
                MaDon = maDon,
                NgayLap = DateOnly.FromDateTime(DateTime.Today),
                NgayKi = DateOnly.FromDateTime(vm.NgayNhan), // dự kiến ký khi nhận xe
                TrangThai = "Chờ ký"
            };

            var don = new DonThueXe
            {
                MaDon = maDon,
                MaKhachHang = khach.MaKhachHang,
                NgayDat = DateTime.Now,
                NgayNhan = vm.NgayNhan.Date,
                NgayTra = vm.NgayTra.Date,
                CocXe = xe.TienCoc ?? 0m,
                TongTien = tongTien,
                TrangThai = "Chờ xác nhận",
                DiaDiemGiaoXe = string.IsNullOrWhiteSpace(vm.DiaDiemGiaoXe) ? null : vm.DiaDiemGiaoXe.Trim(),
                DiaDiemTraXe = string.IsNullOrWhiteSpace(vm.DiaDiemTraXe) ? null : vm.DiaDiemTraXe.Trim(),
                MaDonNavigation = hopDong
            };
            don.ChiTietDonThues.Add(new ChiTietDonThue
            {
                MaDon = maDon,
                MaXe = xe.MaXe,
                DonGiaThue = giaNgay
            });

            _context.DonThueXes.Add(don);

            try
            {
                await _context.SaveChangesAsync(); // 1 transaction: hoặc lưu hết, hoặc không lưu gì
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Không lưu được đơn thuê xe {MaDon}", maDon);
                ModelState.AddModelError(string.Empty, "Hệ thống chưa lưu được đăng ký. Vui lòng thử lại hoặc gọi hotline.");
                return View(vm);
            }

            // Truyền sang trang cảm ơn bằng TempData (không để lộ thông tin khách qua URL)
            TempData["MaDon"] = maDon;
            TempData["HoTen"] = khach.HoTen;
            TempData["TenXe"] = xe.TenXe;
            TempData["NgayNhan"] = vm.NgayNhan.ToString("dd/MM/yyyy");
            TempData["NgayTra"] = vm.NgayTra.ToString("dd/MM/yyyy");
            TempData["TongTien"] = ViewHelper.Tien(tongTien);
            return RedirectToAction(nameof(ThanhCong));
        }

        // 4. Trang cảm ơn sau khi đăng ký
        [HttpGet("dang-ky-thanh-cong")]
        public IActionResult ThanhCong()
        {
            if (TempData["MaDon"] is not string maDon) return RedirectToAction(nameof(Index));

            return View(new DangKyThanhCongViewModel
            {
                MaDon = maDon,
                HoTen = TempData["HoTen"] as string ?? "",
                TenXe = TempData["TenXe"] as string ?? "",
                NgayNhan = TempData["NgayNhan"] as string ?? "",
                NgayTra = TempData["NgayTra"] as string ?? "",
                TongTien = TempData["TongTien"] as string ?? ""
            });
        }
    }
}
