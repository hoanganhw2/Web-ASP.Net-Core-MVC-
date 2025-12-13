using BackEnd.Models;

namespace BackEnd.Service
{
    public interface ICartService
    {
        List<CartItem> GetCart();
        Task AddToCart(int bookId, int quantity = 1);
        void UpdateQuantity(int bookId, int quantity);
        void RemoveFromCart(int bookId);
        void ClearCart();
        int GetCartCount();
        double GetCartTotal();
    }
}
