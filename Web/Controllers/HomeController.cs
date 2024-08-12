using Application.Services;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public HomeController(ILogger<HomeController> logger, ProductService productService, CategoryService categoryService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            List<Category> nonparents = _categoryService.GetNonParentCategories();
            List<Product> products = new List<Product>();
            products = _productService.GetAllProducts().ToList();
            //getting top 10 products in last 7 days 
            products = products.OrderByDescending(p => p.CreatedAt > DateTime.Now.AddDays(-7)).Take(10).ToList();
            ViewBag.Products = products;
            return View(nonparents);
        }

        public IActionResult ShopItems(int? id, int pageNumber, int pageSize, string view)
        {
            List<Category> parents = _categoryService.GetCategoriesHavingSubCategories();
            List<SubCategoryViewModel> subCategoryViewModels = new List<SubCategoryViewModel>();
            List<Product> products = new List<Product>();
            foreach (Category category in parents)
            {
                SubCategoryViewModel subCategory = new SubCategoryViewModel();
                subCategory.Category = category;
                subCategory.SubCategories = _categoryService.GetSubCategories(category.Id);
                subCategory.Products = _productService.GetProductsByCategory(category.Id);
                subCategoryViewModels.Add(subCategory);
            }

            if (id != null)
            {
                products = _productService.GetProductsByCategory((int)id);
                string name = _categoryService.GetCategoryById((int)id).CategoryName;
                if (name != null)
                    TempData["CategoryName"] = name;
            }
            else
            {
                products = _productService.GetAllProducts().ToList();
            }
            ViewBag.Categories = subCategoryViewModels;

            if (pageSize <= 0)
            {
                pageSize = 15;
            }

            int totalItems = products.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (pageNumber < 1)
                pageNumber = 1;
            var pages = new PaginatedList(pageNumber, totalPages, pageSize, totalItems);
            var data = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Pages = pages;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.View = view;

            return View(data);
        }


        public IActionResult ShopProduct(int id)
        {
            //SubCategoryViewModel subCategory = new SubCategoryViewModel();
            //subCategory.GetSubCategories(categoryId);
            //subCategory.GetProducts(categoryId);
            //return View(subCategory);
            Product product = _productService.GetProductById(id);
            return View(product);
        }

        public IActionResult Load(string x)
        {
            return PartialView("_ProductsList", _productService.SearchProducts(x).ToList());
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
