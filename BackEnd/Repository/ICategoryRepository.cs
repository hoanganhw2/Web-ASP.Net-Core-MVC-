using BackEnd.Models.Entity;

namespace BackEnd.Repository
{
    public interface ICategoryRepository
    {
        public Task<IEnumerable<Category>?> getAllCategory();
    }
}
