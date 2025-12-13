using BackEnd.Models.Entity;
using BackEnd.Repository;

namespace BackEnd.Service
{
    public class CategoryService : ICategoryService
    {
        public ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetCategories()
        {
            var categories = await _categoryRepository.getAllCategory();   

            return categories?.ToList() ?? new List<Category>();
        }
    }
}
