using BackEnd.Models;
using System.Text.Json;

namespace BackEnd.Service
{
    public class WishlistService : IWishlistService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBookService _bookService;
        private const string WishlistSessionKey = "Wishlist";

        public WishlistService(IHttpContextAccessor httpContextAccessor, IBookService bookService)
        {
            _httpContextAccessor = httpContextAccessor;
            _bookService = bookService;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public List<WishlistItem> GetWishlist()
        {
            var wishlistJson = Session.GetString(WishlistSessionKey);
            if (string.IsNullOrEmpty(wishlistJson))
            {
                return new List<WishlistItem>();
            }
            return JsonSerializer.Deserialize<List<WishlistItem>>(wishlistJson) ?? new List<WishlistItem>();
        }

        private void SaveWishlist(List<WishlistItem> wishlist)
        {
            var wishlistJson = JsonSerializer.Serialize(wishlist);
            Session.SetString(WishlistSessionKey, wishlistJson);
        }

        public async Task AddToWishlist(int bookId)
        {
            var wishlist = GetWishlist();
            
            // Kiểm tra xem sản phẩm đã có trong wishlist chưa
            if (wishlist.Any(x => x.BookId == bookId))
            {
                return; // Đã có rồi, không thêm nữa
            }

            var book = await _bookService.GetBookById(bookId);
            if (book != null)
            {
                var imagePath = book.Images?.FirstOrDefault()?.Path ?? "";
                wishlist.Add(new WishlistItem
                {
                    BookId = book.Id,
                    BookName = book.Name,
                    Price = book.Price,
                    PriceDiscount = book.Price_Discount,
                    ImagePath = imagePath
                });
                SaveWishlist(wishlist);
            }
        }

        public void RemoveFromWishlist(int bookId)
        {
            var wishlist = GetWishlist();
            var item = wishlist.FirstOrDefault(x => x.BookId == bookId);

            if (item != null)
            {
                wishlist.Remove(item);
                SaveWishlist(wishlist);
            }
        }

        public void ClearWishlist()
        {
            Session.Remove(WishlistSessionKey);
        }

        public int GetWishlistCount()
        {
            return GetWishlist().Count;
        }

        public bool IsInWishlist(int bookId)
        {
            return GetWishlist().Any(x => x.BookId == bookId);
        }
    }
}
