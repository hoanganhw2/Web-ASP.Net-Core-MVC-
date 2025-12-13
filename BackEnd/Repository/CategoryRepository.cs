using BackEnd.Data;
using BackEnd.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public BookDbContext _bookDbContext;

        public CategoryRepository(BookDbContext bookDbContext)
        {
            _bookDbContext = bookDbContext;
        }

        public async Task<IEnumerable<Category>> getAllCategory()
        {
           return await _bookDbContext.Categories.Include(c=>c.subCategories).ToListAsync();
        }
    }
}
