using Application.Services;
using Application.UseCases;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace Web.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly CategoryService _categoryService;
        private readonly BrandService _brandService;
        private readonly GetUnitNameUseCase _unitNameUsecase;
        private readonly GetUnitsUseCase _unitsUseCase;
        private readonly ProductService _productService;

        public ProductController(
            IWebHostEnvironment env,
            CategoryService categoryService, ProductService productService,
            BrandService brandService,
            GetUnitNameUseCase unitNameUsecase, GetUnitsUseCase unitsUseCase)
        {
            _env = env;
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _unitNameUsecase = unitNameUsecase;
            _unitsUseCase = unitsUseCase;
        }

        public async Task<IActionResult> ProductList(string search, int pageNumber)
        {
            List<Product> products;
            if (!string.IsNullOrEmpty(search))
            {
                products = (await _productService.SearchProductsAsync(search)).ToList();
            }
            else
            {
                products = (await _productService.GetAllProductsAsync()).ToList();
            }

            foreach (var product in products)
            {
                product.CategoryName = (await _categoryService.GetCategoryByIdAsync(product.CategoryID)).CategoryName;
                product.BrandName = await _brandService.GetBrandNameAsync(product.BrandID);
            }

            const int pageSize = 5;
            var totalRecords = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            var paginatedProducts = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var pager = new PaginatedList(pageNumber, totalPages, pageSize, totalRecords);

            ViewBag.Pager = pager;
            return View(paginatedProducts);
        }

        public async Task<IActionResult> Details(int id)
        {
            Product? product = await _productService.GetProductByIdAsync(id);

            // Check if the product exists
            if (product == null)
            {
                // Product does not exist, show an alert
                TempData["ProductNotFound"] = "Product not found";
                return RedirectToAction("ProductList", "Product");
            }

            product.CategoryName = (await _categoryService.GetCategoryByIdAsync(product.CategoryID)).CategoryName;
            product.BrandName = await _brandService.GetBrandNameAsync(product.BrandID);
            product.UnitName = await _unitNameUsecase.GetNameAsync(product.UnitID);

            // Showing error message if product already exists
            if (TempData["ProductExists"] != null)
            {
                ViewBag.Alert = TempData["ProductExists"];
            }

            return View(product);
        }

        public async Task<IActionResult> AddProduct(int id)
        {
            var addProduct = new AddProductViewModel();
            if (id != 0)
            {
                var product = await _productService.GetProductByIdAsync(id);

                // Check if the product exists
                if (product == null)
                {
                    // Product does not exist, show an alert
                    TempData["ProductNotFound"] = "Product not found";
                    return RedirectToAction("ProductList", "Product");
                }
                addProduct.Product = product;
            }

            var categories = (await _categoryService.GetNonParentCategoriesAsync()).ToList();
            var brands = (await _brandService.GetAllBrandsAsync()).ToList();
            var units = (await _unitsUseCase.GetAsync()).ToList();

            addProduct.Categories = new SelectList(categories, "Id", "CategoryName");
            addProduct.Brands = new SelectList(brands, "Id", "BrandName");
            addProduct.Units = new SelectList(units, "Id", "Name");

            // Showing error message if product already exists
            if (TempData["ProductExists"] != null)
            {
                ViewBag.Alert = TempData["ProductExists"];
            }

            return View(addProduct);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddProductViewModel p)
        {
            if (ModelState.IsValid)
            {
                if (p.Product?.Picture != null)
                {
                    p.Product.ImagePath = GetPath(p.Product.Picture);
                }

                // Save changes to the repository
                await _productService.UpdateProductAsync(p.Product!);

                return RedirectToAction("ProductList", "Product");
            }
            else
            {
                // If model state is not valid, return to the edit view with validation errors
                return View(p.Product);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            // Check if the product exists
            if (product == null)
            {
                // Product does not exist, show an alert
                TempData["ProductNotFound"] = "Product not found";
                return RedirectToAction("ProductList", "Product");
            }

            await _productService.DeleteProductAsync(id);
            return RedirectToAction("ProductList", "Product");
        }

        private string GetPath(IFormFile picture)
        {
            string wwwrootPath = _env.WebRootPath;
            string path = Path.Combine(wwwrootPath, "images", "products");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string UniqueFileName = Guid.NewGuid().ToString() + "_" + picture.FileName;
            if (picture != null && picture.Length > 0)
            {
                path = Path.Combine(path, UniqueFileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    picture.CopyTo(fileStream);
                }
            }
            return Path.Combine("images", "products", UniqueFileName);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductViewModel? model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product();

                if (model?.Product != null)
                {
                    product = await _productService.GetProductIfExistsAsync(model.Product.Name!, model.Product.CategoryID, model.Product.BrandID);

                    if (product.Id == 0)
                    {
                        // Product does not exist, add new product
                        product = model.Product;
                        product.CreatedAt = string.IsNullOrEmpty(model.Product.CreatedAt.ToString()) ? DateTime.Now : model.Product.CreatedAt;
                        product.UpdatedAt = string.IsNullOrEmpty(model.Product.UpdatedAt.ToString()) ? DateTime.Now : model.Product.UpdatedAt;
                        product.ImagePath = GetPath(model.Product.Picture!);
                        await _productService.CreateProductAsync(product);
                    }
                    else
                    {
                        // Product already exists, show an alert
                        TempData["ProductExists"] = "Product already exists";
                        return RedirectToAction("AddProduct", "Product");
                    }
                }
            }
            else
            {
                TempData["InvalidModel"] = "Invalid product details provided";
                return RedirectToAction("AddProduct", "Product");
            }

            return RedirectToAction("ProductList", "Product");
        }
    }
}
