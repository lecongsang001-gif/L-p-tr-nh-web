using Microsoft.EntityFrameworkCore;
using nhom7thuexe.Models;
using nhom7thuexe.ViewModels;

namespace nhom7thuexe.Helpers;

/// <summary>Truy vấn dùng chung: lấy xe kèm giá đang áp dụng.</summary>
public static class XeQueries
{
    public static async Task<List<XeCardViewModel>> LayDanhSachXeAsync(this AppDbContext db)
    {
        var homNay = DateOnly.FromDateTime(DateTime.Today);
        var xes = await db.Xes.AsNoTracking()
            .Include(x => x.BangGia)
            .OrderBy(x => x.TenXe)
            .ToListAsync();
        return xes.Select(x => XeCardViewModel.Tu(x, homNay)).ToList();
    }

    public static async Task<XeCardViewModel?> LayMotXeAsync(this AppDbContext db, string maXe)
    {
        var homNay = DateOnly.FromDateTime(DateTime.Today);
        var xe = await db.Xes.AsNoTracking()
            .Include(x => x.BangGia)
            .FirstOrDefaultAsync(x => x.MaXe == maXe);
        return xe == null ? null : XeCardViewModel.Tu(xe, homNay);
    }
}
