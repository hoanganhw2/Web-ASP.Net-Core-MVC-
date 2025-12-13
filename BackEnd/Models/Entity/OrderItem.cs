using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.Entity
{
    [Table("OrderItem")]
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        public int BookId { get; set; }
        
        /// <summary>
        /// Lưu tên sách tại thời điểm mua
        /// </summary>
        [Required]
        public string BookName { get; set; } = string.Empty;
        
        /// <summary>
        /// Lưu giá tại thời điểm mua
        /// </summary>
        public double Price { get; set; }
        
        public int Quantity { get; set; }
        
        /// <summary>
        /// Thành tiền = Price * Quantity
        /// </summary>
        public double Total { get; set; }
        
        // Navigation properties
        public Order? Order { get; set; }
        public Book? Book { get; set; }
    }
}
