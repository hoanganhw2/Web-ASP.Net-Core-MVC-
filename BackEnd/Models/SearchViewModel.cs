using BackEnd.Models.Entity;

namespace BackEnd.Models
{
    public class SearchViewModel
    {
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string Keyword { get; set; }
        public int? CategoryId { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
