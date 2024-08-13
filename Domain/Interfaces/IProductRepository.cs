namespace Domain
{
    public interface IProductRepository : IRepository<Product>
    {
        // Task<List<Product>> SearchAsync(string search);
        Task<Product> GetProductAsync(string name, int categoryID, int brandID);
        Task<List<Product>> GetProductsByCategoryAsync(int categoryID);
    }
}