using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Models.Entity;
using BackEnd.Service;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly BookDbContext _dbContext;

        public CheckoutController(ICartService cartService, BookDbContext dbContext)
        {
            _cartService = cartService;
            _dbContext = dbContext;
        }

        // GET: /checkout
        [HttpGet]
        [Route("checkout")]
        public IActionResult Index(string note = "")
        {
            var cart = _cartService.GetCart();
            
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            // Lấy thông tin user nếu đã đăng nhập
            var userId = HttpContext.Session.GetInt32("id");
            if (userId != null)
            {
                var user = _dbContext.Users.Find(userId);
                if (user != null)
                {
                    ViewBag.User = user;
                }
            }

            ViewBag.CartItems = cart;
            ViewBag.CartTotal = _cartService.GetCartTotal();
            ViewBag.ShippingFee = 30000; // Phí ship mặc định
            ViewBag.Note = note; // Ghi chú từ trang giỏ hàng
            
            return View();
        }

        // POST: /checkout/place-order
        [HttpPost]
        [Route("checkout/place-order")]
        public async Task<IActionResult> PlaceOrder(string shippingAddress, string phoneNumber, string note, string discountCode)
        {
            var userId = HttpContext.Session.GetInt32("id");
            
            if (userId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để đặt hàng!";
                return RedirectToAction("Login", "Users");
            }

            var cart = _cartService.GetCart();
            if (cart == null || cart.Count == 0)
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            var subTotal = _cartService.GetCartTotal();
            double discountAmount = 0;
            double shippingFee = 30000;
            int? discountCodeId = null;

            // Kiểm tra mã giảm giá
            if (!string.IsNullOrEmpty(discountCode))
            {
                var discount = _dbContext.DiscountCodes
                    .FirstOrDefault(d => d.Code == discountCode && d.IsActive 
                        && d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now
                        && d.UsedCount < d.UsageLimit
                        && subTotal >= d.MinOrderAmount);

                if (discount != null)
                {
                    discountCodeId = discount.Id;
                    
                    if (discount.DiscountType == 0) // Theo %
                    {
                        discountAmount = subTotal * discount.DiscountValue / 100;
                        if (discountAmount > discount.MaxDiscountAmount)
                        {
                            discountAmount = discount.MaxDiscountAmount;
                        }
                    }
                    else // Số tiền cố định
                    {
                        discountAmount = discount.DiscountValue;
                    }

                    // Tăng số lần sử dụng
                    discount.UsedCount++;
                }
            }

            var totalAmount = subTotal - discountAmount + shippingFee;

            // Tạo đơn hàng
            var order = new Order
            {
                UserId = userId.Value,
                OrderDate = DateTime.Now,
                ShippingAddress = shippingAddress,
                PhoneNumber = phoneNumber,
                Note = note,
                SubTotal = subTotal,
                DiscountAmount = discountAmount,
                ShippingFee = shippingFee,
                TotalAmount = totalAmount,
                Status = 0, // Chờ xác nhận
                DiscountCodeId = discountCodeId
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            // Tạo chi tiết đơn hàng
            foreach (var cartItem in cart)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    BookId = cartItem.BookId,
                    BookName = cartItem.BookName,
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity,
                    Total = cartItem.Total
                };
                _dbContext.OrderItems.Add(orderItem);
            }

            await _dbContext.SaveChangesAsync();

            // Xóa giỏ hàng
            _cartService.ClearCart();

            TempData["Success"] = "Đặt hàng thành công!";
            return RedirectToAction("OrderSuccess", new { orderId = order.Id });
        }

        // GET: /checkout/order-success/{orderId}
        [HttpGet]
        [Route("checkout/order-success/{orderId}")]
        public IActionResult OrderSuccess(int orderId)
        {
            var order = _dbContext.Orders
                .Where(o => o.Id == orderId)
                .Select(o => new {
                    o.Id,
                    o.OrderDate,
                    o.TotalAmount,
                    o.Status
                })
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.Order = order;
            return View();
        }

        // POST: /checkout/apply-discount (AJAX)
        [HttpPost]
        [Route("checkout/apply-discount")]
        public IActionResult ApplyDiscount(string code, double subTotal)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá!" });
            }

            var discount = _dbContext.DiscountCodes
                .FirstOrDefault(d => d.Code == code);

            if (discount == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không tồn tại!" });
            }

            // Kiểm tra các điều kiện
            if (!discount.IsActive)
            {
                return Json(new { success = false, message = "Mã giảm giá đã bị vô hiệu hóa!" });
            }

            if (discount.StartDate > DateTime.Now)
            {
                return Json(new { success = false, message = "Mã giảm giá chưa có hiệu lực!" });
            }

            if (discount.EndDate < DateTime.Now)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết hạn!" });
            }

            if (discount.UsedCount >= discount.UsageLimit)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết lượt sử dụng!" });
            }

            if (subTotal < discount.MinOrderAmount)
            {
                return Json(new { 
                    success = false, 
                    message = $"Đơn hàng tối thiểu {discount.MinOrderAmount:N0}đ để áp dụng mã này!" 
                });
            }

            double discountAmount;
            if (discount.DiscountType == 0) // Theo %
            {
                discountAmount = subTotal * discount.DiscountValue / 100;
                if (discountAmount > discount.MaxDiscountAmount)
                {
                    discountAmount = discount.MaxDiscountAmount;
                }
            }
            else // Số tiền cố định
            {
                discountAmount = discount.DiscountValue;
            }

            return Json(new { 
                success = true, 
                message = discount.Description,
                discountAmount = discountAmount
            });
        }
    }
}
