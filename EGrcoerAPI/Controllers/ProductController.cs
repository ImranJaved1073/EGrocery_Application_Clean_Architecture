using Application.Services;
using Application.UseCases;
using EGrcoerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nelibur.ObjectMapper;

namespace EGrocerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ProductService _productService;

        public ProductController(IWebHostEnvironment env,ProductService productService)
        {
            _env = env;
            _productService = productService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> ProductList()
        {
            var domainProducts = (await _productService.GetAllProductsAsync()).ToList();
            var products = TinyMapper.Map<List<Product>>(domainProducts);
            return Ok(products);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            return Ok(TinyMapper.Map<Product>(product));
        }

        [HttpPut("edit")]
        public async Task<IActionResult> Edit(int id,[FromBody] Product p)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
                return NotFound("Product not found");

            if (id != p.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(TinyMapper.Map<Domain.Product>(p));
                return Ok("Product updated successfully");
            }

            return BadRequest("Invalid product data");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            await _productService.DeleteProductAsync(id);
            return Ok("Product deleted successfully");
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromBody] Product? model)
        {
            if (ModelState.IsValid && model != null)
            {
                var existingProduct = await _productService.GetProductIfExistsAsync(model.Name!, model.CategoryID, model.BrandID);

                if (existingProduct.Id == 0)
                {
                    model.CreatedAt = model.CreatedAt;
                    model.UpdatedAt = model.UpdatedAt;
                    await _productService.CreateProductAsync(TinyMapper.Map<Domain.Product>(model));
                    return Ok("Product added successfully");
                }

                return Conflict("Product already exists");
            }

            return BadRequest("Invalid product details");
        }
    }
}
