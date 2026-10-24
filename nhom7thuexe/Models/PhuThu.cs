using System;
using System.Collections.Generic;

namespace nhom7thuexe.Models;

public partial class PhuThu
{
    public string MaPhuThu { get; set; } = null!;

    public string MaDon { get; set; } = null!;

    public string? MaSuCo { get; set; }

    public string MaNhanVien { get; set; } = null!;

    public string? LoaiPhuThu { get; set; }

    public decimal? SoTien { get; set; }

    public string? LyDo { get; set; }

    public DateTime? NgayGhiNhan { get; set; }

    public string? TrangThai { get; set; }

    public virtual DonThueXe MaDonNavigation { get; set; } = null!;

    public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;

    public virtual SuCoDichVu? MaSuCoNavigation { get; set; }
}
