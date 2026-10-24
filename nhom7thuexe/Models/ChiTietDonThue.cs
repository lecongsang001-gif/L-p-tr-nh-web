using System;
using System.Collections.Generic;

namespace nhom7thuexe.Models;

public partial class ChiTietDonThue
{
    public string MaDon { get; set; } = null!;

    public string MaXe { get; set; } = null!;

    public decimal DonGiaThue { get; set; }

    public decimal? TienPhuThu { get; set; }

    public string? LyDoPhuThu { get; set; }

    public virtual DonThueXe MaDonNavigation { get; set; } = null!;

    public virtual Xe MaXeNavigation { get; set; } = null!;
}
