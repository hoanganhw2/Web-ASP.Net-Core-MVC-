using System.Diagnostics;
using System.Threading.Tasks;
using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Models.Entity;
using BackEnd.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;      
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;
        private readonly BookDbContext _dbContext;



        public HomeController(ILogger<HomeController> logger, IUserService userService, ICategoryService categoryService, IBookService bookService, BookDbContext dbContext)
        {
            _logger = logger;          
            _categoryService = categoryService;
            _bookService = bookService;
            _dbContext = dbContext;

        }
        [Route("/")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetCategories();
            var books = await _bookService.getAllBooks();
            ViewBag.Categories = categories;
            books = books.Take(20).ToList();
            
            // Lấy sách theo từng category
            var booksMoney = await _bookService.getBooksByCategoryId(1);      // Sách Kinh tế
            var booksSpecialized = await _bookService.getBooksByCategoryId(2); // Sách chuyên ngành
            var booksChildren = await _bookService.getBooksByCategoryId(3);    // Sách thiếu nhi
            
            // Lấy top 3 sách bán chạy nhất
            var topSellingBookIds = await _dbContext.OrderItems
                .GroupBy(oi => oi.BookId)
                .Select(g => new { BookId = g.Key, TotalQuantity = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(3)
                .Select(x => x.BookId)
                .ToListAsync();
            
            var topSellingBooks = await _dbContext.Books
                .Include(b => b.Images)
                .Where(b => topSellingBookIds.Contains(b.Id))
                .ToListAsync();
            
            // Sắp xếp lại theo thứ tự bán chạy
            topSellingBooks = topSellingBooks.OrderBy(b => topSellingBookIds.IndexOf(b.Id)).ToList();
            
            ViewBag.Books = books;
            ViewBag.BooksMoney = booksMoney;
            ViewBag.BooksSpecialized = booksSpecialized;
            ViewBag.BooksChildren = booksChildren;
            ViewBag.TopSellingBooks = topSellingBooks;
            return View();
        }
       
       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public async Task<IActionResult> Test()
        {
            var list = await _bookService.getBooksByCategoryId(1);

            return Ok(list);
        }

        /// <summary>
        /// Trang hiển thị khi user không có quyền truy cập (403 Forbidden)
        /// </summary>
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
      
       
        

    }

}
