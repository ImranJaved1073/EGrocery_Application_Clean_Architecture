using Dapper;
using Microsoft.Data.SqlClient;
using NuGet.Protocol;
using Domain;

namespace Infrastructure
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly string _connString;

        public ProductRepository(string connString) : base(connString)
        {
            _connString = connString;
        }

        public override List<Product> Search(string search)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string sql = "SELECT * FROM Product WHERE Name LIKE @search";
                var parameters = new { search = "%" + search + "%" };
                var products = conn.Query<Product>(sql, parameters).AsList();
                return products;
            }
        }

        public Product GetProduct(string name, int categoryID, int brandID)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string sql = "SELECT * FROM Product WHERE Name = @name AND CategoryID = @categoryID AND BrandID = @brandID";
                var parameters = new { name = name, categoryID = categoryID, brandID = brandID };
                var product = conn.QueryFirstOrDefault<Product>(sql, parameters);
                return product ?? new Product();
            }
        }

        public List<Product> GetProductsByCategory(int categoryID)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string sql = "SELECT * FROM Product WHERE CategoryID = @categoryID";
                var parameters = new { categoryID = categoryID };
                var products = conn.Query<Product>(sql, parameters).AsList();
                return products;
            }

        }
    }
}
