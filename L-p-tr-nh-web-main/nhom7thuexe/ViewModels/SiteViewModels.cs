using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using nhom7thuexe.Models;

namespace nhom7thuexe.ViewModels;

/// <summary>Một chiếc xe + giá đang áp dụng (dùng cho thẻ xe và bảng giá).</summary>
public class XeCardViewModel
{
    public string MaXe { get; set; } = "";
    public string TenXe { get; set; } = "";
    public string TenHang { get; set; } = "";
    public string LoaiXeMay { get; set; } = "";
    public short PhanKhoi { get; set; }
    public string? MauSac { get; set; }
    public string? HinhAnh { get; set; }
    public string? TrangThai { get; set; }

    public decimal? GiaTheoNgay { get; set; }
    public decimal? GiaTheoTuan { get; set; }
    public decimal? GiaTheoThang { get; set; }
    public decimal? TienCoc { get; set; }

    public bool SanSang =>
        string.Equals(TrangThai?.Trim(), "Sẵn sàng", StringComparison.OrdinalIgnoreCase);

    public static XeCardViewModel Tu(Xe xe, DateOnly homNay)
    {
        // Ưu tiên bảng giá đang còn hiệu lực; nếu không có thì lấy dòng mới nhất để không bị trống giá.
        var gia = xe.BangGia
                .Where(b => b.ApDungTu <= homNay && (b.ApDungDen == null || b.ApDungDen >= homNay))
                .OrderByDescending(b => b.ApDungTu)
                .FirstOrDefault()
            ?? xe.BangGia.OrderByDescending(b => b.ApDungTu).FirstOrDefault();

        return new XeCardViewModel
        {
            MaXe = xe.MaXe,
            TenXe = xe.TenXe,
            TenHang = xe.TenHang,
            LoaiXeMay = (xe.LoaiXeMay ?? "").Trim(),
            PhanKhoi = xe.PhanKhoi,
            MauSac = xe.MauSac,
            HinhAnh = xe.HinhAnh,
            TrangThai = xe.TrangThai,
            GiaTheoNgay = gia?.GiaTheoNgay,
            GiaTheoTuan = gia?.GiaTheoTuan,
            GiaTheoThang = gia?.GiaTheoThang,
            TienCoc = gia?.TienCoc
        };
    }

    private static readonly string[] ThuTuLoai = { "tay ga", "số", "côn", "phân khối" };

    private static int ThuTu(string loai)
    {
        var l = loai.ToLowerInvariant();
        for (var i = 0; i < ThuTuLoai.Length; i++)
            if (l.Contains(ThuTuLoai[i])) return i;
        return ThuTuLoai.Length;
    }

    /// <summary>Gom xe theo LoaiXeMay (xe tay ga, xe số, xe côn...) giống các mục trên trang thuê xe.</summary>
    public static List<NhomXeViewModel> NhomTheoLoai(IEnumerable<XeCardViewModel> xes) =>
        xes.GroupBy(x => x.LoaiXeMay, StringComparer.OrdinalIgnoreCase)
           .OrderBy(g => ThuTu(g.Key))
           .ThenBy(g => g.Key)
           .Select(g => new NhomXeViewModel { LoaiXeMay = g.Key, Xes = g.ToList() })
           .ToList();
}

public class NhomXeViewModel
{
    public string LoaiXeMay { get; set; } = "";
    public List<XeCardViewModel> Xes { get; set; } = new();
}

public class HomeIndexViewModel
{
    public List<string> LoaiXe { get; set; } = new();
    public List<XeCardViewModel> XeNoiBat { get; set; } = new();
}

/// <summary>Form "ĐĂNG KÝ NGAY": tạo KhachHang (nếu mới) + HopDong + DonThueXe + ChiTietDonThue.</summary>
public class DangKyThueXeViewModel
{
    [Required]
    public string MaXe { get; set; } = "";

    // Chỉ để hiển thị, không nhận từ form
    [BindNever, ValidateNever]
    public XeCardViewModel? Xe { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    [Display(Name = "Họ và tên")]
    public string HoTen { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại gồm 10 chữ số, bắt đầu bằng 0")]
    [Display(Name = "Số điện thoại")]
    public string Sdt { get; set; } = "";

    // Bắt buộc vì KhachHang.Email có unique index: SQL Server chỉ cho phép 1 dòng NULL.
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email chưa đúng định dạng")]
    [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số CCCD")]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "CCCD gồm đúng 12 chữ số")]
    [Display(Name = "Số CCCD")]
    public string Cccd { get; set; } = "";

    [Display(Name = "Tôi đã có giấy phép lái xe (GPLX)")]
    public bool CoGplx { get; set; }

    [StringLength(255)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [Required(ErrorMessage = "Chọn ngày nhận xe")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày nhận xe")]
    public DateTime NgayNhan { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Chọn ngày trả xe")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày trả xe")]
    public DateTime NgayTra { get; set; } = DateTime.Today.AddDays(1);

    [StringLength(255)]
    [Display(Name = "Địa điểm nhận xe")]
    public string? DiaDiemGiaoXe { get; set; }

    [StringLength(255)]
    [Display(Name = "Địa điểm trả xe")]
    public string? DiaDiemTraXe { get; set; }
}

public class DangKyThanhCongViewModel
{
    public string MaDon { get; set; } = "";
    public string HoTen { get; set; } = "";
    public string TenXe { get; set; } = "";
    public string NgayNhan { get; set; } = "";
    public string NgayTra { get; set; } = "";
    public string TongTien { get; set; } = "";
}
