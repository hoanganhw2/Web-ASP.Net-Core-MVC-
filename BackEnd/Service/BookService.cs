using BackEnd.Models.Entity;
using BackEnd.Repository;

namespace BackEnd.Service

{
    public class BookService : IBookService
    {
        private IBookReopository _bookReopository;
        public BookService(IBookReopository bookReopository)
        {
            _bookReopository = bookReopository;
        }
        public async Task<IEnumerable<Book>> getAllBooks()
        {
            return await _bookReopository.getAllBooks();
        }

        public async Task<Book?> GetBookById(int? id)
        {
            return await _bookReopository.getBookById(id);
        }
       public async Task<IEnumerable<Book>> getBooksByCategory(int subcategoryId)
       {
           return await _bookReopository.getBooksByCategory(subcategoryId);
       }
       public async Task<IEnumerable<Book>> getBooksByCategoryId(int categoryId)
       {
           return await _bookReopository.getBooksByCategoryId(categoryId);
       }
       
       public async Task<IEnumerable<Book>> GetRelatedBooks(int bookId, int subcategoryId, int take = 4)
       {
           return await _bookReopository.getRelatedBooks(bookId, subcategoryId, take);
       }
       
       public async Task<IEnumerable<Book>> GetFeaturedBooks(int take = 5)
       {
           return await _bookReopository.getFeaturedBooks(take);
       }
    }
}
