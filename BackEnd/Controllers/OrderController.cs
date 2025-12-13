using BackEnd.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    public class OrderController : Controller
    {
        private readonly BookDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(BookDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: /orders
        [HttpGet]
        [Route("orders")]
        public async Task<IActionResult> Index()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("id");
            
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var orders = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.DiscountCode)
                .Where(o => o.UserId == userId.Value)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // GET: /orders/details/5
        [HttpGet]
        [Route("orders/details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("id");
            
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var order = await _dbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                        .ThenInclude(b => b.Images)
                .Include(o => o.DiscountCode)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId.Value);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: /orders/cancel/5
        [HttpPost]
        [Route("orders/cancel/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("id");
            
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId.Value);

            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
            }

            // Chỉ có thể hủy đơn hàng đang chờ xác nhận (status = 0)
            if (order.Status != 0)
            {
                return Json(new { success = false, message = "Không thể hủy đơn hàng này!" });
            }

            order.Status = 3; // Đã hủy
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Đã hủy đơn hàng thành công!" });
        }

        // Helper method to get status text
        public static string GetStatusText(int status)
        {
            return status switch
            {
                0 => "Chờ xác nhận",
                1 => "Đang giao",
                2 => "Hoàn thành",
                3 => "Đã hủy",
                _ => "Không xác định"
            };
        }

        public static string GetStatusBadgeClass(int status)
        {
            return status switch
            {
                0 => "bg-warning",
                1 => "bg-info",
                2 => "bg-success",
                3 => "bg-danger",
                _ => "bg-secondary"
            };
        }
    }
}
