using System;
using System.Collections.Generic;

namespace nhom7thuexe.Models;

public partial class KhachHang
{
    public string MaKhachHang { get; set; } = null!;

    public string? CapDoVip { get; set; }

    public string HoTen { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public string? Email { get; set; }

    public string Cccd { get; set; } = null!;

    public bool? Gplx { get; set; }

    public string? DiaChi { get; set; }

    public int? MaTaiKhoan { get; set; }

    public virtual ICollection<DonThueXe> DonThueXes { get; set; } = new List<DonThueXe>();

    public virtual TaiKhoan? MaTaiKhoanNavigation { get; set; }
}
