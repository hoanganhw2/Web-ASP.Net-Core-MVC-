using BackEnd.Service;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: /cart
        [HttpGet]
        [Route("cart")]
        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            ViewBag.CartTotal = _cartService.GetCartTotal();
            return View(cart);
        }

        // POST: /cart/add
        [HttpPost]
        [Route("cart/add")]
        public async Task<IActionResult> AddToCart(int bookId, int quantity = 1, string returnUrl = "/")
        {
            await _cartService.AddToCart(bookId, quantity);
            return Redirect(returnUrl);
        }

        // POST: /cart/add-ajax (AJAX - JSON response)
        [HttpPost]
        [Route("cart/add-ajax")]
        public async Task<IActionResult> AddToCartAjax(int bookId, int quantity = 1)
        {
            await _cartService.AddToCart(bookId, quantity);
            var cart = _cartService.GetCart();
            var item = cart.FirstOrDefault(x => x.BookId == bookId);
            
            return Json(new
            {
                success = true,
                message = "Đã thêm sản phẩm vào giỏ hàng!",
                itemName = item?.BookName ?? "",
                itemQuantity = item?.Quantity ?? quantity,
                cartTotal = _cartService.GetCartTotal(),
                cartCount = _cartService.GetCartCount()
            });
        }

        // GET: /cart/dropdown-html (AJAX - HTML response for cart dropdown)
        [HttpGet]
        [Route("cart/dropdown-html")]
        public IActionResult GetCartDropdownHtml()
        {
            var cart = _cartService.GetCart();
            ViewBag.CartTotal = _cartService.GetCartTotal();
            return PartialView("_CartDropdown", cart);
        }

        // POST: /cart/update (form submit - redirect)
        [HttpPost]
        [Route("cart/update")]
        public IActionResult UpdateQuantity(int bookId, int quantity)
        {
            _cartService.UpdateQuantity(bookId, quantity);
            return RedirectToAction("Index");
        }

        // POST: /cart/update-ajax (AJAX - JSON response)
        [HttpPost]
        [Route("cart/update-ajax")]
        public IActionResult UpdateQuantityAjax(int bookId, int quantity)
        {
            _cartService.UpdateQuantity(bookId, quantity);
            var cart = _cartService.GetCart();
            var item = cart.FirstOrDefault(x => x.BookId == bookId);
            
            return Json(new
            {
                success = true,
                itemTotal = item?.Total ?? 0,
                itemQuantity = item?.Quantity ?? 1,
                cartTotal = _cartService.GetCartTotal(),
                cartCount = _cartService.GetCartCount()
            });
        }

        // POST: /cart/remove
        [HttpPost]
        [Route("cart/remove")]
        public IActionResult RemoveFromCart(int bookId)
        {
            _cartService.RemoveFromCart(bookId);
            return RedirectToAction("Index");
        }

        // POST: /cart/remove-ajax (AJAX - JSON response)
        [HttpPost]
        [Route("cart/remove-ajax")]
        public IActionResult RemoveFromCartAjax(int bookId)
        {
            _cartService.RemoveFromCart(bookId);
            return Json(new
            {
                success = true,
                message = "Đã xóa sản phẩm khỏi giỏ hàng!",
                cartTotal = _cartService.GetCartTotal(),
                cartCount = _cartService.GetCartCount()
            });
        }

        // POST: /cart/clear
        [HttpPost]
        [Route("cart/clear")]
        public IActionResult ClearCart()
        {
            _cartService.ClearCart();
            return RedirectToAction("Index");
        }
    }
}
