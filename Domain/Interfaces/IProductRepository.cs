namespace Domain
{
    public interface IProductRepository: IRepository<Product>
    {
        //List<Product> Search(string search);
        Product GetProduct(string name, int categoryID, int brandID);
        List<Product> GetProductsByCategory(int categoryID);
    }
}
