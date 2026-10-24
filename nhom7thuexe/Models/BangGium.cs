using System;
using System.Collections.Generic;

namespace nhom7thuexe.Models;

public partial class BangGium
{
    public int MaBangGia { get; set; }

    public string MaXe { get; set; } = null!;

    public decimal GiaTheoNgay { get; set; }

    public decimal GiaTheoTuan { get; set; }

    public decimal GiaTheoThang { get; set; }

    public decimal TienCoc { get; set; }

    public DateOnly ApDungTu { get; set; }

    public DateOnly? ApDungDen { get; set; }

    public virtual Xe MaXeNavigation { get; set; } = null!;
}
