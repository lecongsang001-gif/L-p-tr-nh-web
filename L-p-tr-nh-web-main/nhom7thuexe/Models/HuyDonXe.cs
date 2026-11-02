using System;
using System.Collections.Generic;

namespace nhom7thuexe.Models;

public partial class HuyDonXe
{
    public int MaHuyDon { get; set; }

    public string MaDon { get; set; } = null!;

    public string? MaNhanVien { get; set; }

    public DateTime NgayHuy { get; set; }

    public string LyDoHuy { get; set; } = null!;

    public decimal? TienHoanTra { get; set; }

    public decimal? TienPhatHuy { get; set; }

    public string? TrangThaiHuy { get; set; }

    public virtual DonThueXe MaDonNavigation { get; set; } = null!;

    public virtual NhanVien? MaNhanVienNavigation { get; set; }
}
