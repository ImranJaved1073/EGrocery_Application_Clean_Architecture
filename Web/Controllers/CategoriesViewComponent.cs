using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        public CategoriesViewComponent(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        public IViewComponentResult Invoke()
        {
            List<Category> parents = _categoryRepository.GetCategoriesWithSubCategories();
            List<SubCategoryViewModel> subCategoryViewModels = new List<SubCategoryViewModel>();
            foreach (Category category in parents)
            {
                SubCategoryViewModel subCategory = new SubCategoryViewModel();
                subCategory.Category = category;
                subCategory.SubCategories = _categoryRepository.GetSubCategories(category.Id);
                subCategory.Products = _productRepository.GetProductsByCategory(category.Id);
                subCategoryViewModels.Add(subCategory);
            }
            return View(subCategoryViewModels);
        }


    }
}
