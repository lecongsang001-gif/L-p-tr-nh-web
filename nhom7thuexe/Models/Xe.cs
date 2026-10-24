using System;
using System.Collections.Generic;

namespace nhom7thuexe.Models;

public partial class Xe
{
    public string MaXe { get; set; } = null!;

    public string TenXe { get; set; } = null!;

    public string TenHang { get; set; } = null!;

    public string LoaiXeMay { get; set; } = null!;

    public short PhanKhoi { get; set; }

    public short? NamSanXuat { get; set; }

    public string? MauSac { get; set; }

    public string? HinhAnh { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<BangGium> BangGia { get; set; } = new List<BangGium>();

    public virtual ICollection<ChiTietDonThue> ChiTietDonThues { get; set; } = new List<ChiTietDonThue>();

    public virtual ICollection<SuCoDichVu> SuCoDichVus { get; set; } = new List<SuCoDichVu>();
}
