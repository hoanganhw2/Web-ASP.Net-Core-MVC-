namespace BackEnd.Models
{
    public class CartItem
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string ImagePath { get; set; }

        public double Total => Price * Quantity;
    }
}
