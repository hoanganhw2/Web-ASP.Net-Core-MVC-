using BackEnd.Models.Entity;

namespace BackEnd.Service
{
    public interface ICategoryService
    {
        public Task<List<Category>> GetCategories();
    }
}
