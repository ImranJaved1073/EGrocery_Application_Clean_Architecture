using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public List<Category> GetAll()
        {
           return _categoryRepository.Get();
        }

        public List<Category> SearchCategory(string keyword)
        {
            return _categoryRepository.Search(keyword);
        }

        public List<Category> GetParentCategories()
        {
            return _categoryRepository.GetParents();
        }

        public List<Category> GetNames()
        {
            return _categoryRepository.GetNames();
        }

        public Category GetCategoryById(int id)
        {
            return _categoryRepository.Get(id);
        }

        public void AddCategory(Category category)
        {
            _categoryRepository.Add(category);
        }

        public void RemoveCategory(int id)
        {
            _categoryRepository.Delete(id);
        }

        public void UpdateCategory(Category category)
        {
            _categoryRepository.Update(category);
        }

        public List<Category> GetCategoriesHavingSubCategories()
        {
           return _categoryRepository.GetCategoriesWithSubCategories();
        }

        public List<Category> GetNonParentCategories()
        {
            return _categoryRepository.GetNonParentCategories();
        }

        public List<Category> GetSubCategories(int parentCategoryId)
        {
            return _categoryRepository.GetSubCategories(parentCategoryId);
        }
    }
}
