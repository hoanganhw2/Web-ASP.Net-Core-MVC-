using BackEnd.Models;
using System.Text.Json;

namespace BackEnd.Service
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBookService _bookService;
        private const string CartSessionKey = "ShoppingCart";

        public CartService(IHttpContextAccessor httpContextAccessor, IBookService bookService)
        {
            _httpContextAccessor = httpContextAccessor;
            _bookService = bookService;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public List<CartItem> GetCart()
        {
            var cartJson = Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            Session.SetString(CartSessionKey, cartJson);
        }

        public async Task AddToCart(int bookId, int quantity = 1)
        {
            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(x => x.BookId == bookId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var book = await _bookService.GetBookById(bookId);
                if (book != null)
                {
                    var imagePath = book.Images?.FirstOrDefault()?.Path ?? "";
                    cart.Add(new CartItem
                    {
                        BookId = book.Id,
                        BookName = book.Name,
                        Price = book.Price_Discount,
                        Quantity = quantity,
                        ImagePath = imagePath
                    });
                }
            }

            SaveCart(cart);
        }

        public void UpdateQuantity(int bookId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.BookId == bookId);

            if (item != null)
            {
                // Số lượng tối thiểu là 1, không cho giảm xuống 0
                if (quantity < 1)
                {
                    quantity = 1;
                }
                item.Quantity = quantity;
                SaveCart(cart);
            }
        }

        public void RemoveFromCart(int bookId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.BookId == bookId);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
        }

        public void ClearCart()
        {
            Session.Remove(CartSessionKey);
        }

        public int GetCartCount()
        {
            // Đếm số loại sản phẩm khác nhau trong giỏ, không phải tổng số lượng
            return GetCart().Count;
        }

        public double GetCartTotal()
        {
            return GetCart().Sum(x => x.Total);
        }
    }
}
