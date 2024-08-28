using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EGrcoerAPI.Models;

using Nelibur.ObjectMapper;

namespace EGrocerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;

        public CategoryController(CategoryService categoryService, ProductService productService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var domainCategories = await _categoryService.GetAllAsync();
            var categories = TinyMapper.Map<List<Category>>(domainCategories);
            return Ok(categories);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var domainCategory = await _categoryService.GetCategoryByIdAsync(id);

            if (domainCategory == null)
            {
                return NotFound();
            }

            var category = TinyMapper.Map<Category>(domainCategory);
            return Ok(category);
        }

        [HttpGet("getDetails/{id}")]
        public async Task<ActionResult<Category>> GetCategoryInfo(int id)
        {
            var domainCategory = await _categoryService.GetCategoryByIdAsync(id);

            if (domainCategory == null)
            {
                return NotFound();
            }

            var subCategories = await _categoryService.GetSubCategoriesAsync(domainCategory.Id);
            var products = await _productService.GetProductsByCategoryAsync(domainCategory.Id);


            var categoryDetails = new SubCategoryViewModel
            {
                Category = TinyMapper.Map<Category>(domainCategory),
                SubCategories = TinyMapper.Map<List<Category>>(subCategories),
                Products = TinyMapper.Map<List<Product>>(products)
            };
            return Ok(categoryDetails);
        }

        [HttpGet("getDetails")]
        public async Task<ActionResult<Category>> GetCategoryInfo()
        {
            var domainCategories = await _categoryService.GetAllAsync();

            if (domainCategories == null)
            {
                return NotFound();
            }

            List<SubCategoryViewModel> subCategoryViewModels = new List<SubCategoryViewModel>();

            foreach (var category in domainCategories) {
                var subCategory = new Domain.SubCategoryViewModel
                {
                    Category = category,
                    SubCategories = await _categoryService.GetSubCategoriesAsync(category.Id),
                    Products = await _productService.GetProductsByCategoryAsync(category.Id)
                };
                subCategoryViewModels.Add(TinyMapper.Map<SubCategoryViewModel>(subCategory));
            }

            return Ok(subCategoryViewModels);

        }


        [HttpPost("add")]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _categoryService.AddCategoryAsync(TinyMapper.Map<Domain.Category>(category));

            return Ok();
        }


        [HttpPut("edit/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            Domain.Category c = await _categoryService.GetCategoryByIdAsync(id);

            if (c == null)
            {
                return NotFound();
            }

            await _categoryService.UpdateCategoryAsync(TinyMapper.Map<Domain.Category>(category));

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Domain.Category c = await _categoryService.GetCategoryByIdAsync(id);

            if (c == null)
            {
                return NotFound();
            }
            await _categoryService.RemoveCategoryAsync(id);
            return NoContent();
        }

        [HttpGet("subCategory/{id}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetSubCategories(int id)
        {
            List<Domain.Category> subCategories = await _categoryService.GetSubCategoriesAsync(id);

            if (subCategories == null)
            {
                return NotFound();
            }

            return Ok(subCategories);
        }

        [HttpGet("independent")]
        public async Task<ActionResult<IEnumerable<Category>>> GetNonParentCategories()
        {
            var nonParentCategories = await _categoryService.GetNonParentCategoriesAsync();
            return Ok(nonParentCategories);
        }

        [HttpGet("getParents")]
        public async Task<ActionResult<IEnumerable<Category>>> GetParentCategories()
        {
            var nonParentCategories = await _categoryService.GetParentCategoriesAsync();
            return Ok(nonParentCategories);
        }
    }
}
