using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BackEnd.Models.Entity
{
    [Table("Book")]
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }   
        public double Price_Discount{ get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public int subCategoryId { get; set; }
        public SubCategory subCategory { get; set; }
        
        public ICollection<Image> Images { get; set; }
    }
}