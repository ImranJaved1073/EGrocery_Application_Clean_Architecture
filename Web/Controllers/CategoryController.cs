using Application.Services;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public IActionResult List(string search, int pageNumber)
        {
            List<Category> categories = new();
            if (!string.IsNullOrEmpty(search))
            {
                categories = _categoryService.SearchCategory(search).ToList();
            }
            else
            {
                categories = _categoryService.GetParentCategories().ToList();
            }
            foreach (var category in categories)
            {
                category.ProductCount = _productService.GetProductsByCategory(category.Id).Count();
            }
            const int pageSize = 5;
            //int excludeRecords = (pageSize * pageNumer) - pageSize;
            //var totalRecords = categories.Count;
            //var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            //categories = categories.Skip(excludeRecords).Take(pageSize).ToList();
            //ViewBag.TotalPages = totalPages;
            //ViewBag.PageNumber = pageNumer;
            //return View(categories);

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

        public IActionResult Create(int id)
        {
            List<Category> categories = _categoryService.GetNames().ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            if (id > 0)
            {
                return View(_categoryService.GetCategoryById(id));
            }
            return View();

        }

        [HttpPost]
        public IActionResult Create(Category c)
        {
            if (c.CategoryImg != null)
                c.ImgPath = GetPath(c.CategoryImg);
            _categoryService.AddCategory(c);
            return RedirectToAction("List", "Category");
        }

        private string GetPath(IFormFile picture)
        {
            string wwwrootPath = _env.WebRootPath;
            string path = Path.Combine(wwwrootPath, "images", "categories");
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
            return Path.Combine("images", "categories", UniqueFileName);
        }


        public IActionResult Delete(int id)
        {
            //Category c = categoryRepository.Get(id);
            _categoryService.RemoveCategory(id);
            return RedirectToAction("List", "Category");
        }

        //public IActionResult Edit(int id)
        //{
        //    CategoryRepository categoryRepository = new CategoryRepository();
        //    List<Category> categories = categoryRepository.GetNames().ToList();
        //    ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
        //    return View(categoryRepository.Get(id));
        //}

        [HttpPost]

        public IActionResult Edit(Category c)
        {
            if (c.CategoryImg != null)
                c.ImgPath = GetPath(c.CategoryImg);
            _categoryService.UpdateCategory(c);
            return RedirectToAction("List", "Category");
        }

        [HttpPost]
        public IActionResult CreateOrEdit(Category category)
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
                    _categoryService.AddCategory(category);
                }
                else
                {
                    if (files.Count > 0)
                    {
                        category.ImgPath = GetPath(files[0]);
                    }
                    _categoryService.UpdateCategory(category);
                }
            }
            return Json(new { success = true, message = "Saved Successfully" });

        }

    }
}
