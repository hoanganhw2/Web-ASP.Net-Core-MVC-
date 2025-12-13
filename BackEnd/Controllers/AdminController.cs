using BackEnd.Attributes;
using BackEnd.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    /// <summary>
    /// Admin Controller - Chỉ ADMIN mới có quyền truy cập
    /// </summary>
    [RoleAuthorize("ADMIN")]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly BookDbContext _dbContext;

        public AdminController(BookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Trang Dashboard - Tổng quan thống kê
        /// </summary>
        [HttpGet]
        [Route("")]
        [Route("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            // Thống kê tổng quan
            ViewBag.TotalOrders = await _dbContext.Orders.CountAsync();
            ViewBag.TotalBooks = await _dbContext.Books.CountAsync();
            ViewBag.TotalCategories = await _dbContext.Categories.CountAsync();
            ViewBag.TotalUsers = await _dbContext.Users.CountAsync();

            // Thống kê đơn hàng theo trạng thái
            ViewBag.PendingOrders = await _dbContext.Orders.CountAsync(o => o.Status == 0);
            ViewBag.ShippingOrders = await _dbContext.Orders.CountAsync(o => o.Status == 1);
            ViewBag.CompletedOrders = await _dbContext.Orders.CountAsync(o => o.Status == 2);
            ViewBag.CancelledOrders = await _dbContext.Orders.CountAsync(o => o.Status == 3);

            // Doanh thu
            ViewBag.TotalRevenue = await _dbContext.Orders
                .Where(o => o.Status == 2)
                .SumAsync(o => o.TotalAmount);

            // Đơn hàng mới nhất
            var recentOrders = await _dbContext.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();
            ViewBag.RecentOrders = recentOrders;

            return View();
        }

        #region Category Management
        
        /// <summary>
        /// Danh sách danh mục
        /// </summary>
        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> Categories()
        {
            var categories = await _dbContext.Categories
                .Include(c => c.subCategories)
                .ToListAsync();
            return View(categories);
        }

        /// <summary>
        /// Thêm danh mục mới
        /// </summary>
        [HttpPost]
        [Route("categories/add")]
        public async Task<IActionResult> AddCategory(string name, int displayOrder)
        {
            if (string.IsNullOrEmpty(name))
            {
                TempData["Error"] = "Tên danh mục không được để trống!";
                return RedirectToAction("Categories");
            }

            var category = new Models.Entity.Category
            {
                Name = name,
                DislayOrder = displayOrder
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Thêm danh mục thành công!";
            return RedirectToAction("Categories");
        }

        /// <summary>
        /// Cập nhật danh mục
        /// </summary>
        [HttpPost]
        [Route("categories/update")]
        public async Task<IActionResult> UpdateCategory(int id, string name, int displayOrder)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Không tìm thấy danh mục!" });
            }

            category.Name = name;
            category.DislayOrder = displayOrder;
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật thành công!" });
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        [HttpPost]
        [Route("categories/delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Không tìm thấy danh mục!" });
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Xóa danh mục thành công!" });
        }

        #endregion

        #region Product Management

        /// <summary>
        /// Danh sách sản phẩm
        /// </summary>
        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> Products()
        {
            var products = await _dbContext.Books
                .Include(b => b.subCategory)
                    .ThenInclude(s => s.Category)
                .Include(b => b.Images)
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            ViewBag.SubCategories = await _dbContext.SubCategories
                .Include(s => s.Category)
                .ToListAsync();

            return View(products);
        }

        /// <summary>
        /// Chi tiết sản phẩm để chỉnh sửa
        /// </summary>
        [HttpGet]
        [Route("products/edit/{id}")]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _dbContext.Books
                .Include(b => b.Images)
                .Include(b => b.subCategory)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.SubCategories = await _dbContext.SubCategories
                .Include(s => s.Category)
                .ToListAsync();

            return View(product);
        }

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        [HttpPost]
        [Route("products/update")]
        public async Task<IActionResult> UpdateProduct(int id, string name, string title, 
            double price, double priceDiscount, int quantity, string description, int subCategoryId)
        {
            var product = await _dbContext.Books.FindAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });
            }

            product.Name = name;
            product.Title = title;
            product.Price = price;
            product.Price_Discount= priceDiscount;
            product.Stock= quantity;
            product.Description = description;
            product.subCategoryId = subCategoryId;

            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật sản phẩm thành công!" });
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        [HttpPost]
        [Route("products/delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _dbContext.Books
                .Include(b => b.Images)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });
            }

            // Xóa ảnh liên quan
            _dbContext.Images.RemoveRange(product.Images);
            _dbContext.Books.Remove(product);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
        }

        /// <summary>
        /// Trang thêm sản phẩm mới
        /// </summary>
        [HttpGet]
        [Route("products/add")]
        public async Task<IActionResult> AddProduct()
        {
            ViewBag.SubCategories = await _dbContext.SubCategories
                .Include(s => s.Category)
                .ToListAsync();
            return View();
        }

        /// <summary>
        /// Xử lý thêm sản phẩm mới
        /// </summary>
        [HttpPost]
        [Route("products/add")]
        public async Task<IActionResult> AddProduct(string name, string title, double price, 
            double priceDiscount, int stock, string description, int subCategoryId, IFormFile? imageFile)
        {
            if (string.IsNullOrEmpty(name))
            {
                TempData["Error"] = "Tên sản phẩm không được để trống!";
                return RedirectToAction("AddProduct");
            }

            var product = new Models.Entity.Book
            {
                Name = name,
                Title = title,
                Price = price,
                Price_Discount = priceDiscount,
                Stock = stock,
                Description = description,
                subCategoryId = subCategoryId
            };

            _dbContext.Books.Add(product);
            await _dbContext.SaveChangesAsync();

            // Xử lý upload ảnh
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = $"{product.Id}_{DateTime.Now.Ticks}{Path.GetExtension(imageFile.FileName)}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "product");
                
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                var image = new Models.Entity.Image
                {
                    Path = fileName,
                    product_id = product.Id
                };
                _dbContext.Images.Add(image);
                await _dbContext.SaveChangesAsync();
            }

            TempData["Success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("Products");
        }

        #endregion

        #region DiscountCode Management

        /// <summary>
        /// Danh sách mã giảm giá
        /// </summary>
        [HttpGet]
        [Route("discounts")]
        public async Task<IActionResult> Discounts()
        {
            var discounts = await _dbContext.DiscountCodes
                .OrderByDescending(d => d.Id)
                .ToListAsync();
            return View(discounts);
        }

        /// <summary>
        /// Thêm mã giảm giá mới
        /// </summary>
        [HttpPost]
        [Route("discounts/add")]
        public async Task<IActionResult> AddDiscount(string code, string description, int discountType,
            double discountValue, double minOrderAmount, double maxDiscountAmount, int usageLimit,
            DateTime startDate, DateTime endDate, bool isActive)
        {
            if (string.IsNullOrEmpty(code))
            {
                TempData["Error"] = "Mã giảm giá không được để trống!";
                return RedirectToAction("Discounts");
            }

            // Kiểm tra mã đã tồn tại chưa
            var existingCode = await _dbContext.DiscountCodes.FirstOrDefaultAsync(d => d.Code == code);
            if (existingCode != null)
            {
                TempData["Error"] = "Mã giảm giá đã tồn tại!";
                return RedirectToAction("Discounts");
            }

            var discount = new Models.Entity.DiscountCode
            {
                Code = code.ToUpper(),
                Description = description,
                DiscountType = discountType,
                DiscountValue = discountValue,
                MinOrderAmount = minOrderAmount,
                MaxDiscountAmount = maxDiscountAmount,
                UsageLimit = usageLimit,
                UsedCount = 0,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = isActive
            };

            _dbContext.DiscountCodes.Add(discount);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Thêm mã giảm giá thành công!";
            return RedirectToAction("Discounts");
        }

        /// <summary>
        /// Cập nhật mã giảm giá
        /// </summary>
        [HttpPost]
        [Route("discounts/update")]
        public async Task<IActionResult> UpdateDiscount(int id, string code, string description, int discountType,
            double discountValue, double minOrderAmount, double maxDiscountAmount, int usageLimit,
            DateTime startDate, DateTime endDate, bool isActive)
        {
            var discount = await _dbContext.DiscountCodes.FindAsync(id);
            if (discount == null)
            {
                return Json(new { success = false, message = "Không tìm thấy mã giảm giá!" });
            }

            discount.Code = code.ToUpper();
            discount.Description = description;
            discount.DiscountType = discountType;
            discount.DiscountValue = discountValue;
            discount.MinOrderAmount = minOrderAmount;
            discount.MaxDiscountAmount = maxDiscountAmount;
            discount.UsageLimit = usageLimit;
            discount.StartDate = startDate;
            discount.EndDate = endDate;
            discount.IsActive = isActive;

            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật thành công!" });
        }

        /// <summary>
        /// Xóa mã giảm giá
        /// </summary>
        [HttpPost]
        [Route("discounts/delete/{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var discount = await _dbContext.DiscountCodes.FindAsync(id);
            if (discount == null)
            {
                return Json(new { success = false, message = "Không tìm thấy mã giảm giá!" });
            }

            _dbContext.DiscountCodes.Remove(discount);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Xóa mã giảm giá thành công!" });
        }

        /// <summary>
        /// Toggle trạng thái mã giảm giá
        /// </summary>
        [HttpPost]
        [Route("discounts/toggle/{id}")]
        public async Task<IActionResult> ToggleDiscount(int id)
        {
            var discount = await _dbContext.DiscountCodes.FindAsync(id);
            if (discount == null)
            {
                return Json(new { success = false, message = "Không tìm thấy mã giảm giá!" });
            }

            discount.IsActive = !discount.IsActive;
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = discount.IsActive ? "Đã kích hoạt mã!" : "Đã vô hiệu hóa mã!" });
        }

        #endregion

        #region Order Management

        /// <summary>
        /// Danh sách đơn hàng
        /// </summary>
        [HttpGet]
        [Route("orders")]
        public async Task<IActionResult> Orders(int? status = null)
        {
            var query = _dbContext.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.DiscountCode)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewBag.CurrentStatus = status;
            return View(orders);
        }

        /// <summary>
        /// Chi tiết đơn hàng
        /// </summary>
        [HttpGet]
        [Route("orders/details/{id}")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                        .ThenInclude(b => b.Images)
                .Include(o => o.DiscountCode)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng
        /// </summary>
        [HttpPost]
        [Route("orders/update-status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, int status)
        {
            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
            }

            order.Status = status;
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật trạng thái thành công!" });
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Trang thống kê chi tiết
        /// </summary>
        [HttpGet]
        [Route("statistics")]
        public async Task<IActionResult> Statistics()
        {
            // Doanh thu theo tháng (12 tháng gần nhất)
            var monthlyRevenue = await _dbContext.Orders
                .Where(o => o.Status == 2 && o.OrderDate >= DateTime.Now.AddMonths(-12))
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            ViewBag.MonthlyRevenue = monthlyRevenue;

            // Top sản phẩm bán chạy
            var topProducts = await _dbContext.OrderItems
                .GroupBy(oi => new { oi.BookId, oi.BookName })
                .Select(g => new
                {
                    BookId = g.Key.BookId,
                    BookName = g.Key.BookName,
                    TotalQuantity = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Total)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(10)
                .ToListAsync();

            ViewBag.TopProducts = topProducts;

            return View();
        }

        #endregion

        #region User Management

        /// <summary>
        /// Danh sách người dùng
        /// </summary>
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> Users()
        {
            var users = await _dbContext.Users
                .Include(u => u.role)
                .OrderByDescending(u => u.id)
                .ToListAsync();

            ViewBag.Roles = await _dbContext.Roles.ToListAsync();
            return View(users);
        }

        /// <summary>
        /// Chi tiết người dùng
        /// </summary>
        [HttpGet]
        [Route("users/details/{id}")]
        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _dbContext.Users
                .Include(u => u.role)
                .FirstOrDefaultAsync(u => u.id == id);

            if (user == null)
            {
                return NotFound();
            }

            // Lấy đơn hàng của người dùng
            var orders = await _dbContext.Orders
                .Where(o => o.UserId == id)
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToListAsync();

            ViewBag.Orders = orders;
            ViewBag.Roles = await _dbContext.Roles.ToListAsync();

            return View(user);
        }

        /// <summary>
        /// Cập nhật role người dùng
        /// </summary>
        [HttpPost]
        [Route("users/update-role")]
        public async Task<IActionResult> UpdateUserRole(int userId, int roleId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng!" });
            }

            user.role_id = roleId;
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật role thành công!" });
        }

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        [HttpPost]
        [Route("users/delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng!" });
            }

            // Kiểm tra không cho xóa chính mình
            var currentUserId = HttpContext.Session.GetInt32("id");
            if (currentUserId == id)
            {
                return Json(new { success = false, message = "Không thể xóa tài khoản của chính mình!" });
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Xóa người dùng thành công!" });
        }

        #endregion
    }
}
