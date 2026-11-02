using System;
using System.Collections.Generic;

namespace nhom7thuexe.Models;

public partial class TaiKhoanNhanVien
{
    public string MaTaiKhoan { get; set; } = null!;

    public string MaNhanVien { get; set; } = null!;

    public string TaiKhoan { get; set; } = null!;

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public DateTime? NgayTao { get; set; }

    public DateTime? DangNhapLanCuoi { get; set; }

    public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;
}
