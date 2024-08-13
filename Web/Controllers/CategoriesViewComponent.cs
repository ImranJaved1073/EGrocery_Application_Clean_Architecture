using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var parents = await _categoryRepository.GetCategoriesWithSubCategoriesAsync();
            var subCategoryViewModels = new List<SubCategoryViewModel>();

            foreach (var category in parents)
            {
                var subCategory = new SubCategoryViewModel
                {
                    Category = category,
                    SubCategories = await _categoryRepository.GetSubCategoriesAsync(category.Id),
                    Products = await _productRepository.GetProductsByCategoryAsync(category.Id)
                };
                subCategoryViewModels.Add(subCategory);
            }

            return View(subCategoryViewModels);
        }
    }
}
