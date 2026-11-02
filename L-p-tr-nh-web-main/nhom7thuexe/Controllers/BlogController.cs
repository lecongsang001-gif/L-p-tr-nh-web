using Microsoft.AspNetCore.Mvc;

namespace nhom7thuexe.Controllers
{
    // Database chưa có bảng bài viết nên Blog tạm thời là trang tĩnh.
    // Khi cần lưu bài viết, thêm bảng BaiViet rồi đọc dữ liệu ở đây.
    public class BlogController : Controller
    {
        [HttpGet("blog")]
        public IActionResult Index(string? muc)
        {
            ViewData["TieuDe"] = muc switch
            {
                "kinh-nghiem" => "Kinh nghiệm thuê xe",
                "tin-tuc" => "Tin tức",
                _ => "Blog"
            };
            return View();
        }
    }
}
