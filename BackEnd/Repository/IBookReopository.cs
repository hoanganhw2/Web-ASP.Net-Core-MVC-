using BackEnd.Models.Entity;

namespace BackEnd.Repository
{
    public interface IBookReopository
    {
        Task<IEnumerable<Book>> getAllBooks();
        Task<IEnumerable<Book>> getBooksByCategory(int subcategoryId);
        Task<Book?> getBookById(int? id);
        Task<IEnumerable<Book>> getBooksByCategoryId(int categoryId);
        Task<IEnumerable<Book>> getRelatedBooks(int bookId, int subcategoryId, int take = 4);
        Task<IEnumerable<Book>> getFeaturedBooks(int take = 5);
        
    }
}
