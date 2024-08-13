using Dapper;
using Microsoft.Data.SqlClient;
using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly string _connString;

        public ProductRepository(string connString) : base(connString)
        {
            _connString = connString;
        }

        public override async Task<List<Product>> SearchAsync(string search)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string sql = "SELECT * FROM Product WHERE Name LIKE @search";
                var parameters = new { search = "%" + search + "%" };
                var products = (await conn.QueryAsync<Product>(sql, parameters)).AsList();
                return products;
            }
        }

        public async Task<Product> GetProductAsync(string name, int categoryID, int brandID)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string sql = "SELECT * FROM Product WHERE Name = @name AND CategoryID = @categoryID AND BrandID = @brandID";
                var parameters = new { name = name, categoryID = categoryID, brandID = brandID };
                var product = await conn.QueryFirstOrDefaultAsync<Product>(sql, parameters);
                return product ?? new Product();
            }
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryID)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string sql = "SELECT * FROM Product WHERE CategoryID = @categoryID";
                var parameters = new { categoryID = categoryID };
                var products = (await conn.QueryAsync<Product>(sql, parameters)).AsList();
                return products;
            }
        }
    }
}
