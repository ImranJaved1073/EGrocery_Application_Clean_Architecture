using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAsync();
        }

        public async Task<List<Category>> SearchCategoryAsync(string keyword)
        {
            return await _categoryRepository.SearchAsync(keyword);
        }

        public async Task<List<Category>> GetParentCategoriesAsync()
        {
            return await _categoryRepository.GetParentsAsync();
        }

        public async Task<List<Category>> GetNamesAsync()
        {
            return await _categoryRepository.GetNamesAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetAsync(id);
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _categoryRepository.AddAsync(category);
        }

        public async Task RemoveCategoryAsync(int id)
        {
            await _categoryRepository.DeleteAsync(id);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await _categoryRepository.UpdateAsync(category);
        }

        public async Task<List<Category>> GetCategoriesHavingSubCategoriesAsync()
        {
            return await _categoryRepository.GetCategoriesWithSubCategoriesAsync();
        }

        public async Task<List<Category>> GetNonParentCategoriesAsync()
        {
            return await _categoryRepository.GetNonParentCategoriesAsync();
        }

        public async Task<List<Category>> GetSubCategoriesAsync(int parentCategoryId)
        {
            return await _categoryRepository.GetSubCategoriesAsync(parentCategoryId);
        }
    }
}
