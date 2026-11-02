using System;
using System.Collections.Generic;

namespace nhom7thuexe.Models;

public partial class HopDong
{
    public string MaHopDong { get; set; } = null!;

    public string MaDon { get; set; } = null!;

    public DateOnly NgayLap { get; set; }

    public DateOnly NgayKi { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual DonThueXe? DonThueXe { get; set; }
}
