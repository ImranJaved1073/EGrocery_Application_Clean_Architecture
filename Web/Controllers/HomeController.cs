using Application.Services;
using Application.UseCases;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly GetUnitNameUseCase _unitNameUsecase;

        public HomeController(ILogger<HomeController> logger, ProductService productService, CategoryService categoryService, GetUnitNameUseCase unitNameUseCase)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
            _unitNameUsecase = unitNameUseCase;
        }

        public async Task<IActionResult> Index()
        {
            var nonparents = await _categoryService.GetNonParentCategoriesAsync();
            var products = (await _productService.GetAllProductsAsync()).ToList();
            // Getting top 10 products in last 7 days
            products = products.Where(p => p.CreatedAt > DateTime.Now.AddDays(-7))
                                .OrderByDescending(p => p.CreatedAt)
                                .Take(10)
                                .ToList();
            ViewBag.Products = products;
            return View(nonparents);
        }

        public async Task<IActionResult> ShopItems(int? id, int pageNumber, int pageSize, string view)
        {
            var parents = await _categoryService.GetCategoriesHavingSubCategoriesAsync();
            var subCategoryViewModels = new List<SubCategoryViewModel>();
            List<Product> products;

            foreach (var category in parents)
            {
                var subCategory = new SubCategoryViewModel
                {
                    Category = category,
                    SubCategories = await _categoryService.GetSubCategoriesAsync(category.Id),
                    Products = await _productService.GetProductsByCategoryAsync(category.Id)
                };
                subCategoryViewModels.Add(subCategory);
            }

            if (id != null)
            {
                products = (await _productService.GetProductsByCategoryAsync((int)id)).ToList();
                var name = (await _categoryService.GetCategoryByIdAsync((int)id)).CategoryName;
                if (name != null)
                    TempData["CategoryName"] = name;
            }
            else
            {
                products = (await _productService.GetAllProductsAsync()).ToList();
            }
            ViewBag.Categories = subCategoryViewModels;

            if (pageSize <= 0)
            {
                pageSize = 15;
            }

            var totalItems = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (pageNumber < 1)
                pageNumber = 1;

            var pager = new PaginatedList(pageNumber, totalPages, pageSize, totalItems);
            var data = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Pages = pager;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.View = view;

            return View(data);
        }

        public async Task<IActionResult> ShopProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            product.UnitName = await _unitNameUsecase.GetNameAsync(product.UnitID);
            return View(product);
        }

        public async Task<IActionResult> Load(string x)
        {
            var products = (await _productService.SearchProductsAsync(x)).ToList();
            return PartialView("_ProductsList", products);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult ContactUS()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
