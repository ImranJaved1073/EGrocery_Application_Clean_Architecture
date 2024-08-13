using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetAsync(id);
        }

        public async Task CreateProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _productRepository.GetProductsByCategoryAsync(categoryId);
        }

        public async Task<List<Product>> SearchProductsAsync(string search)
        {
            return await _productRepository.SearchAsync(search);
        }

        public async Task<Product> GetProductIfExistsAsync(string name, int categoryID, int brandID)
        {
            return await _productRepository.GetProductAsync(name, categoryID, brandID);
        }
    }
}
