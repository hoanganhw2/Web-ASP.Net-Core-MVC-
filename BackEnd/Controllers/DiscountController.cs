using BackEnd.Attributes;
using BackEnd.Data;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    public class DiscountController : Controller
    {
        private readonly BookDbContext _dbContext;

        public DiscountController(BookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /discounts
       
        [HttpGet]
        [Route("discounts")]
        public IActionResult Index()
        {
            var now = DateTime.Now;
            var discounts = _dbContext.DiscountCodes
                .Where(d => d.IsActive && d.EndDate >= now && d.UsedCount < d.UsageLimit)
                .OrderByDescending(d => d.DiscountValue)
                .ToList();
            
            return View(discounts);
        }
    }
}
