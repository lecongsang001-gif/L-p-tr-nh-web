using System;
using System.Collections.Generic;

namespace nhom7thuexe.Models;

public partial class DonThueXe
{
    public string MaDon { get; set; } = null!;

    public string MaKhachHang { get; set; } = null!;

    public string? MaNhanVien { get; set; }

    public DateTime? NgayDat { get; set; }

    public DateTime NgayNhan { get; set; }

    public DateTime NgayTra { get; set; }

    public decimal? CocXe { get; set; }

    public string? TrangThai { get; set; }

    public decimal? TongTien { get; set; }

    public string? DiaDiemGiaoXe { get; set; }

    public string? DiaDiemTraXe { get; set; }

    public virtual ICollection<ChiTietDonThue> ChiTietDonThues { get; set; } = new List<ChiTietDonThue>();

    public virtual ICollection<HuyDonXe> HuyDonXes { get; set; } = new List<HuyDonXe>();

    public virtual HopDong MaDonNavigation { get; set; } = null!;

    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;

    public virtual NhanVien? MaNhanVienNavigation { get; set; }

    public virtual ICollection<PhuThu> PhuThus { get; set; } = new List<PhuThu>();

    public virtual ICollection<SuCoDichVu> SuCoDichVus { get; set; } = new List<SuCoDichVu>();

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
