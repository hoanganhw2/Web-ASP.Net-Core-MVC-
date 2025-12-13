namespace BackEnd.Models
{
    public class WishlistItem
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public double Price { get; set; }
        public double PriceDiscount { get; set; }
        public string ImagePath { get; set; }
    }
}
