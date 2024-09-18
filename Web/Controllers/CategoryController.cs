using Application.Services;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;
        private readonly IMemoryCache _cache;

        public CategoryController(IWebHostEnvironment env, CategoryService categoryService, ProductService productService, IMemoryCache cache)
        {
            _env = env;
            _categoryService = categoryService;
            _productService = productService;
            _cache = cache;
        }

        public async Task<IActionResult> List(string search, int pageNumber)
        {
            List<Category>? categories;
            string cacheKey = $"CategoryList_{search}";

            if (!_cache.TryGetValue(cacheKey, out categories))
            {
                if (!string.IsNullOrEmpty(search))
                {
                    categories = (await _categoryService.SearchCategoryAsync(search)).ToList();
                }
                else
                {
                    categories = (await _categoryService.GetParentCategoriesAsync()).ToList();
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                };

                _cache.Set(cacheKey, categories, cacheEntryOptions);
            }

            foreach (var category in categories!)
            {
                category.ProductCount = (await _productService.GetProductsByCategoryAsync(category.Id)).Count();
            }

            const int pageSize = 5;
            var rescCount = categories.Count();
            var totalPages = (int)Math.Ceiling((double)rescCount / pageSize);

            if (pageNumber < 1)
                pageNumber = totalPages;

            var pager = new PaginatedList(pageNumber, totalPages, pageSize, rescCount);
            var data = categories.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Pager = pager;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;

            return View(data);
        }

        public async Task<IActionResult> Create(int id)
        {
            var categories = await GetCategoriesFromCacheAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");

            if (id > 0)
            {
                return View(await _categoryService.GetCategoryByIdAsync(id));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category c)
        {
            if (c.CategoryImg != null)
                c.ImgPath = GetPath(c.CategoryImg);

            await _categoryService.AddCategoryAsync(c);

            // Invalidate the category list cache after adding a new category
            _cache.Remove("CategoryList_");

            return RedirectToAction("List", "Category");
        }

        private string GetPath(IFormFile picture)
        {
            string wwwrootPath = _env.WebRootPath;
            string path = Path.Combine(wwwrootPath, "images", "categories");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + picture.FileName;
            if (picture != null && picture.Length > 0)
            {
                path = Path.Combine(path, uniqueFileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    picture.CopyTo(fileStream);
                }
            }
            return Path.Combine("images", "categories", uniqueFileName);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.RemoveCategoryAsync(id);

            // Invalidate the category list cache after deleting a category
            _cache.Remove("CategoryList_");

            return RedirectToAction("List", "Category");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category c)
        {
            if (c.CategoryImg != null)
                c.ImgPath = GetPath(c.CategoryImg);

            await _categoryService.UpdateCategoryAsync(c);

            // Invalidate the category list cache after updating a category
            //_cache.Remove("CategoryList_");

            return RedirectToAction("List", "Category");
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrEdit(Category category)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (category.Id == 0)
                {
                    if (files.Count > 0)
                    {
                        category.ImgPath = GetPath(files[0]);
                    }
                    await _categoryService.AddCategoryAsync(category);
                }
                else
                {
                    if (files.Count > 0)
                    {
                        category.ImgPath = GetPath(files[0]);
                    }
                    await _categoryService.UpdateCategoryAsync(category);
                }

                // Invalidate the category list cache after creating or editing a category
                _cache.Remove("CategoryList_");
            }
            return Json(new { success = true, message = "Saved Successfully" });
        }

        private async Task<IEnumerable<Category>> GetCategoriesFromCacheAsync()
        {
            if (!_cache.TryGetValue("Categories", out IEnumerable<Category>? categories))
            {
                categories = (await _categoryService.GetNamesAsync()).ToList();
                _cache.Set("Categories", categories, TimeSpan.FromMinutes(30));
            }
            return categories!;
        }
    }
}
