using System;
using System.Collections.Generic;

namespace nhom7thuexe.Models;

public partial class ThanhToan
{
    public string MaThanhToan { get; set; } = null!;

    public string MaDon { get; set; } = null!;

    public decimal? TienCoc { get; set; }

    public decimal SoTien { get; set; }

    public string? PhuongThuc { get; set; }

    public DateTime? NgayThanhToan { get; set; }

    public string? TrangThai { get; set; }

    public virtual DonThueXe MaDonNavigation { get; set; } = null!;
}
