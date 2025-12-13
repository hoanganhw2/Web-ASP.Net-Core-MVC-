using BackEnd.Service;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // POST: /wishlist/add-ajax
        [HttpPost]
        [Route("wishlist/add-ajax")]
        public async Task<IActionResult> AddToWishlistAjax(int bookId)
        {
            await _wishlistService.AddToWishlist(bookId);
            var wishlist = _wishlistService.GetWishlist();
            var item = wishlist.FirstOrDefault(x => x.BookId == bookId);
            
            return Json(new
            {
                success = true,
                message = "Đã thêm vào danh sách yêu thích!",
                itemName = item?.BookName ?? "",
                wishlistCount = _wishlistService.GetWishlistCount()
            });
        }

        // POST: /wishlist/remove-ajax
        [HttpPost]
        [Route("wishlist/remove-ajax")]
        public IActionResult RemoveFromWishlistAjax(int bookId)
        {
            _wishlistService.RemoveFromWishlist(bookId);
            
            return Json(new
            {
                success = true,
                message = "Đã xóa khỏi danh sách yêu thích!",
                wishlistCount = _wishlistService.GetWishlistCount()
            });
        }

        // GET: /wishlist/dropdown-html
        [HttpGet]
        [Route("wishlist/dropdown-html")]
        public IActionResult GetWishlistDropdownHtml()
        {
            var wishlist = _wishlistService.GetWishlist();
            return PartialView("_WishlistDropdown", wishlist);
        }

        // GET: /wishlist/check
        [HttpGet]
        [Route("wishlist/check")]
        public IActionResult CheckInWishlist(int bookId)
        {
            return Json(new
            {
                isInWishlist = _wishlistService.IsInWishlist(bookId)
            });
        }
    }
}
