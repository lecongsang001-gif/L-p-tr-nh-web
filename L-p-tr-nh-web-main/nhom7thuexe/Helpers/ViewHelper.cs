using System.Globalization;

namespace nhom7thuexe.Helpers;

public static class ViewHelper
{
    private static readonly CultureInfo Vi = new("vi-VN");

    /// <summary>80000 -> "80.000đ"; chưa có giá -> "Liên hệ".</summary>
    public static string Tien(decimal? soTien) =>
        soTien.HasValue && soTien.Value > 0
            ? soTien.Value.ToString("N0", Vi) + "đ"
            : "Liên hệ";

    /// <summary>
    /// Cột Xe.HinhAnh trong DB lưu tên file (vd "lead.jpg") -> ảnh nằm ở wwwroot/images/xe/.
    /// Nếu đã là đường dẫn đầy đủ (bắt đầu bằng / hoặc http) thì giữ nguyên. Chưa có ảnh -> ảnh mặc định.
    /// </summary>
    public static string HinhXe(string? hinhAnh)
    {
        if (string.IsNullOrWhiteSpace(hinhAnh)) return "/images/xe/no-image.svg";
        var h = hinhAnh.Trim();
        if (h.StartsWith('/') || h.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return h;
        return "/images/xe/" + h;
    }
}
