using Application.Services;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public CategoryController(IWebHostEnvironment env, CategoryService categoryService, ProductService productService)
        {
            _env = env;
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> List(string search, int pageNumber)
        {
            List<Category> categories;
            if (!string.IsNullOrEmpty(search))
            {
                categories = (await _categoryService.SearchCategoryAsync(search)).ToList();
            }
            else
            {
                categories = (await _categoryService.GetParentCategoriesAsync()).ToList();
            }
            foreach (var category in categories)
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
            var categories = (await _categoryService.GetNamesAsync()).ToList();
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
            return RedirectToAction("List", "Category");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category c)
        {
            if (c.CategoryImg != null)
                c.ImgPath = GetPath(c.CategoryImg);
            await _categoryService.UpdateCategoryAsync(c);
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
            }
            return Json(new { success = true, message = "Saved Successfully" });
        }
    }
}
