using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public IEnumerable<Product> GetAllProducts()
        {
            return _productRepository.Get();
        }

        public Product GetProductById(int id)
        {
            return _productRepository.Get(id);
        }

        public void CreateProduct(Product product)
        {
            _productRepository.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            _productRepository.Update(product);
        }

        public void DeleteProduct(int id)
        {
            _productRepository.Delete(id);
        }

        public List<Product> GetProductsByCategory(int categoryId)
        {
            return _productRepository.GetProductsByCategory(categoryId);
        }

        public List<Product> SearchProducts(string search)
        {
            return _productRepository.Search(search);
        }

        public Product GetProductIfExists(string name, int categoryID, int brandID)
        {
            return _productRepository.GetProduct(name, categoryID, brandID);
        }
    }
}
