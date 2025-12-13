
using BackEnd.Models.Entity;
namespace BackEnd.Service;

public interface IBookService
{
    Task<IEnumerable<Book>> getAllBooks();
    Task<Book?> GetBookById(int? id);
    Task<IEnumerable<Book>> getBooksByCategory(int subcategoryId);
    Task<IEnumerable<Book>> getBooksByCategoryId(int categoryId);
    Task<IEnumerable<Book>> GetRelatedBooks(int bookId, int subcategoryId, int take = 4);
    Task<IEnumerable<Book>> GetFeaturedBooks(int take = 5);
}
