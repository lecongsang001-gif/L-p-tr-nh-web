namespace nhom7thuexe.Models;

/// <summary>Thông tin thương hiệu, đọc từ mục "SiteInfo" trong appsettings.json.</summary>
public class SiteInfo
{
    public string TenThuongHieu { get; set; } = "";
    public string Hotline { get; set; } = "";
    public string Email { get; set; } = "";
    public string DiaChi { get; set; } = "";
    public string GioLamViec { get; set; } = "";

    /// <summary>Hotline chỉ còn chữ số, dùng cho liên kết tel: (vd "0985.977.793" -> "0985977793").</summary>
    public string HotlineTel => new string((Hotline ?? "").Where(char.IsDigit).ToArray());
}
