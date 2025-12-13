using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.Entity
{
    [Table("Role")]
    public class Role
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public ICollection<User> users { get; set; }

    }
}
