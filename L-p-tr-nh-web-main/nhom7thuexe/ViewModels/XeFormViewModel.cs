using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace nhom7thuexe.ViewModels;

/// <summary>Form THÊM / SỬA xe (gồm cả giá đang áp dụng). Dùng chung cho trang quản lý.</summary>
public class XeFormViewModel
{
    // Danh mục cố định cho dropdown (LoaiXeMay khớp với ThuTuLoai trong XeCardViewModel)
    public static readonly string[] LoaiXeOptions = { "Xe tay ga", "Xe số", "Xe côn", "Xe phân khối" };
    public static readonly string[] TrangThaiOptions = { "Sẵn sàng", "Đang thuê", "Bảo trì" };

    /// <summary>true = đang thêm mới, false = đang sửa (chỉ để hiển thị, không tin dữ liệu từ form).</summary>
    [BindNever]
    public bool LaThem { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã xe")]
    [StringLength(20, ErrorMessage = "Mã xe tối đa 20 ký tự")]
    [RegularExpression(@"^[A-Za-z0-9_\-]+$", ErrorMessage = "Mã xe chỉ gồm chữ, số, gạch ngang hoặc gạch dưới")]
    [Display(Name = "Mã xe")]
    public string MaXe { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập tên xe")]
    [StringLength(100, ErrorMessage = "Tên xe tối đa 100 ký tự")]
    [Display(Name = "Tên xe")]
    public string TenXe { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập hãng xe")]
    [StringLength(50, ErrorMessage = "Hãng xe tối đa 50 ký tự")]
    [Display(Name = "Hãng xe")]
    public string TenHang { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn loại xe")]
    [Display(Name = "Loại xe")]
    public string LoaiXeMay { get; set; } = "";

    [Range(0, 2000, ErrorMessage = "Phân khối từ 0 đến 2000 cc")]
    [Display(Name = "Phân khối (cc)")]
    public short PhanKhoi { get; set; }

    [StringLength(30, ErrorMessage = "Màu sắc tối đa 30 ký tự")]
    [Display(Name = "Màu sắc")]
    public string? MauSac { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = "Sẵn sàng";

    /// <summary>Tên file ảnh hiện tại (lấy từ DB, chỉ để hiển thị).</summary>
    [BindNever]
    public string? HinhAnh { get; set; }

    [Display(Name = "Ảnh xe")]
    public IFormFile? FileAnh { get; set; }

    // ----- Giá đang áp dụng -----
    [Required(ErrorMessage = "Vui lòng nhập giá thuê theo ngày")]
    [Range(1000, 100000000, ErrorMessage = "Giá theo ngày không hợp lệ")]
    [Display(Name = "Giá theo ngày (đ)")]
    public decimal? GiaTheoNgay { get; set; }

    [Range(0, 1000000000, ErrorMessage = "Giá theo tuần không hợp lệ")]
    [Display(Name = "Giá theo tuần (đ)")]
    public decimal? GiaTheoTuan { get; set; }

    [Range(0, 1000000000, ErrorMessage = "Giá theo tháng không hợp lệ")]
    [Display(Name = "Giá theo tháng (đ)")]
    public decimal? GiaTheoThang { get; set; }

    [Range(0, 1000000000, ErrorMessage = "Tiền cọc không hợp lệ")]
    [Display(Name = "Tiền cọc (đ)")]
    public decimal? TienCoc { get; set; }
}
