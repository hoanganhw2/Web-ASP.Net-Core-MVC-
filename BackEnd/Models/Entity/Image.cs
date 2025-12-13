using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.Entity
{
    [Table("Book_Image")]
    public class Image
    {
        [Key]
        public int Id { get; set; }
 
        public string Path { get; set; }
        
        public int  product_id { get; set; }
        public Book Book { get; set; }
    }
}
