using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.Entity
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        public DateTime OrderDate { get; set; }
        
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        
        public string? Note { get; set; }
        
        /// <summary>
        /// Tổng tiền hàng (trước giảm giá)
        /// </summary>
        public double SubTotal { get; set; }
        
        /// <summary>
        /// Số tiền được giảm
        /// </summary>
        public double DiscountAmount { get; set; }
        
        /// <summary>
        /// Phí vận chuyển
        /// </summary>
        public double ShippingFee { get; set; }
        
        /// <summary>
        /// Tổng thanh toán = SubTotal - DiscountAmount + ShippingFee
        /// </summary>
        public double TotalAmount { get; set; }
        
        /// <summary>
        /// Trạng thái: 0 = Chờ xác nhận, 1 = Đang giao, 2 = Hoàn thành, 3 = Đã hủy
        /// </summary>
        public int Status { get; set; }
        
        public int? DiscountCodeId { get; set; }
        
        // Navigation properties
        public User? User { get; set; }
        public DiscountCode? DiscountCode { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
