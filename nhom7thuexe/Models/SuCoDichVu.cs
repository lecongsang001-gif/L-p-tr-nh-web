using System;
using System.Collections.Generic;

namespace nhom7thuexe.Models;

public partial class SuCoDichVu
{
    public string MaSuCo { get; set; } = null!;

    public string? MaDon { get; set; }

    public string? MaXe { get; set; }

    public string? LoaiSuCo { get; set; }

    public string? MoTa { get; set; }

    public decimal? ChiPhiPhatSinh { get; set; }

    public DateTime? NgayGhiNhan { get; set; }

    public virtual DonThueXe? MaDonNavigation { get; set; }

    public virtual Xe? MaXeNavigation { get; set; }

    public virtual ICollection<PhuThu> PhuThus { get; set; } = new List<PhuThu>();
}
