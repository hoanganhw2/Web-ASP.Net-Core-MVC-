using BackEnd.Data;
using BackEnd.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    public class ProductsController : Controller
    {
        public readonly IBookService _bookService;
        public readonly BookDbContext _dbContext;
        
        public ProductsController(IBookService bookService, BookDbContext dbContext)
        {
            _bookService = bookService;
            _dbContext = dbContext;
        }
        
        // GET: /products or /products?subcategory=1&sort=price-asc&page=1
        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> Index(int? subcategory, int? category, string sort = "newest", int page = 1, int pageSize = 12, double? minPrice = null, double? maxPrice = null)
        {
            // Lấy tất cả categories và subcategories cho sidebar
            var categories = await _dbContext.Categories
                .Include(c => c.subCategories)
                .OrderBy(c => c.DislayOrder)
                .ToListAsync();
            ViewBag.Categories = categories;
            
            // Query sản phẩm
            var query = _dbContext.Books
                .Include(b => b.Images)
                .Include(b => b.subCategory)
                .AsQueryable();
            
            // Lọc theo subcategory
            if (subcategory.HasValue && subcategory.Value > 0)
            {
                query = query.Where(b => b.subCategoryId == subcategory.Value);
                var subCat = await _dbContext.SubCategories.FindAsync(subcategory.Value);
                ViewBag.CurrentSubcategory = subCat;
            }
            
            // Lọc theo category
            if (category.HasValue && category.Value > 0)
            {
                var subCategoryIds = await _dbContext.SubCategories
                    .Where(s => s.CategoryId == category.Value)
                    .Select(s => s.Id)
                    .ToListAsync();
                query = query.Where(b => subCategoryIds.Contains(b.subCategoryId));
                var cat = await _dbContext.Categories.FindAsync(category.Value);
                ViewBag.CurrentCategory = cat;
            }
            
            // Lọc theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(b => b.Price_Discount >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(b => b.Price_Discount <= maxPrice.Value);
            }
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            
            // Sắp xếp
            query = sort switch
            {
                "price-asc" => query.OrderBy(b => b.Price_Discount),
                "price-desc" => query.OrderByDescending(b => b.Price_Discount),
                "name-asc" => query.OrderBy(b => b.Name),
                "name-desc" => query.OrderByDescending(b => b.Name),
                "discount" => query.OrderByDescending(b => (b.Price - b.Price_Discount) / b.Price * 100),
                _ => query.OrderByDescending(b => b.Id) // newest
            };
            
            // Đếm tổng số sản phẩm
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            
            // Phân trang
            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            ViewBag.Books = books;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentSort = sort;
            ViewBag.SubcategoryId = subcategory;
            ViewBag.CategoryId = category;
            
            return View();
        }
        
        [HttpGet]
        [Route("products/{id}")]
        public async Task<IActionResult> Details(int id)
        {         
            var book = await _bookService.GetBookById(id);
            ViewBag.Book = book;
            
            // Lấy sản phẩm liên quan (cùng subcategory)
            if (book != null)
            {
                var relatedBooks = await _bookService.GetRelatedBooks(book.Id, book.subCategoryId, 4);
                ViewBag.RelatedBooks = relatedBooks;
            }
            
            // Lấy sản phẩm nổi bật (có % giảm giá cao nhất)
            var featuredBooks = await _bookService.GetFeaturedBooks(5);
            ViewBag.FeaturedBooks = featuredBooks;
            
            return View();
        }

        // GET: /products/search-ajax?q=keyword
        [HttpGet]
        [Route("products/search-ajax")]
        public async Task<IActionResult> SearchAjax(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            {
                return Json(new { success = true, products = new List<object>() });
            }

            var products = await _dbContext.Books
                .Include(b => b.Images)
                .Where(b => b.Name.Contains(q))
                .Take(8)
                .Select(b => new
                {
                    id = b.Id,
                    name = b.Name,
                    price = b.Price,
                    priceDiscount = b.Price_Discount,
                    image = b.Images.FirstOrDefault() != null ? b.Images.FirstOrDefault().Path : null
                })
                .ToListAsync();

            return Json(new { success = true, products = products });
        }
    
    }
}
