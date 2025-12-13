using BackEnd.Data;
using BackEnd.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repository
{
    public class BookRepository : IBookReopository
    {
        public BookDbContext _bookDbContet{ get; set; }
        public BookRepository(BookDbContext bookDbContext)
        {
            _bookDbContet = bookDbContext;
        }
        public async Task<IEnumerable<Book>> getAllBooks()
        {
            return await _bookDbContet.Books.Include(b=>b.Images).OrderByDescending(b => b.Id).ToListAsync();
        }

        public async Task<Book?> getBookById(int? id)
        {
            return await _bookDbContet.Books.Include(b=>b.Images).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> getBooksByCategory(int subcategoryId)
        {
            return await _bookDbContet.Books.Include(b => b.Images).Where(b=>b.subCategoryId == subcategoryId).ToListAsync();
        }

        public async Task<IEnumerable<Book>> getBooksByCategoryId(int categoryId)
        {
            return await _bookDbContet.Books
                .Include(b => b.Images)
                .Include(b => b.subCategory)
                .Where(b => b.subCategory.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> getRelatedBooks(int bookId, int subcategoryId, int take = 4)
        {
            return await _bookDbContet.Books
                .Include(b => b.Images)
                .Where(b => b.subCategoryId == subcategoryId && b.Id != bookId)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> getFeaturedBooks(int take = 5)
        {
            return await _bookDbContet.Books
                .Include(b => b.Images)
                .Where(b => b.Price > 0 && b.Price_Discount > 0)
                .OrderByDescending(b => (b.Price - b.Price_Discount) / b.Price)
                .Take(take)
                .ToListAsync();
        }
        
    }
}
