using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.Entity
{
    [Table("DiscountCode")]
    public class DiscountCode
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Loại giảm giá: 0 = Giảm theo %, 1 = Giảm số tiền cố định
        /// </summary>
        public int DiscountType { get; set; }
        
        /// <summary>
        /// Giá trị giảm (% hoặc số tiền)
        /// </summary>
        public double DiscountValue { get; set; }
        
        /// <summary>
        /// Đơn hàng tối thiểu để áp dụng
        /// </summary>
        public double MinOrderAmount { get; set; }
        
        /// <summary>
        /// Số tiền giảm tối đa (áp dụng khi giảm theo %)
        /// </summary>
        public double MaxDiscountAmount { get; set; }
        
        /// <summary>
        /// Số lần sử dụng tối đa
        /// </summary>
        public int UsageLimit { get; set; }
        
        /// <summary>
        /// Số lần đã sử dụng
        /// </summary>
        public int UsedCount { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        
        // Navigation properties
        public ICollection<Order>? Orders { get; set; }
    }
}

