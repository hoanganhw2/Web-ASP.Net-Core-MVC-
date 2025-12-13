using BackEnd.Models;

namespace BackEnd.Service
{
    public interface IWishlistService
    {
        List<WishlistItem> GetWishlist();
        Task AddToWishlist(int bookId);
        void RemoveFromWishlist(int bookId);
        void ClearWishlist();
        int GetWishlistCount();
        bool IsInWishlist(int bookId);
    }
}
