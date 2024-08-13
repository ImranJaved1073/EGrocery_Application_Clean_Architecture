using Dapper;
using Microsoft.Data.SqlClient;
using Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly string _connectionString;
        public CategoryRepository(string connString) : base(connString)
        {
            _connectionString = connString;
        }

        public async Task<List<Category>> GetNamesAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, CategoryName, CategoryDescription, ImgPath, CreatedOn FROM Category";
                var names = await connection.QueryAsync<Category>(query);
                return names.ToList();
            }
        }

        public async Task<List<Category>> GetParentsAsync()
        {
            string query = @"SELECT 
                            c.Id,
                            c.CategoryName,
                            c.CategoryDescription,
                            c.ImgPath,
                            c.CreatedOn,
                            c.ParentCategoryID,
                            pc.CategoryName AS ParentCategoryName
                          FROM 
                            Category c
                          LEFT JOIN 
                            Category pc ON c.ParentCategoryID = pc.Id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var parents = await connection.QueryAsync<Category>(query);
                return parents.ToList();
            }
        }

        public async Task<List<Category>> GetCategoriesWithSubCategoriesAsync()
        {
            var query = @"SELECT * FROM Category WHERE ParentCategoryID is NULL";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var subCategories = await connection.QueryAsync<Category>(query);
                return subCategories.ToList();
            }
        }

        public async Task<List<Category>> GetSubCategoriesAsync(int parentCategoryId)
        {
            var query = @"SELECT 
                    c.Id,
                    c.CategoryName,
                    c.CategoryDescription,
                    c.ImgPath,
                    c.CreatedOn,
                    c.ParentCategoryID,
                    pc.CategoryName AS ParentCategoryName
                  FROM 
                    Category c
                  LEFT JOIN 
                    Category pc ON c.ParentCategoryID = pc.Id
                  WHERE 
                    c.ParentCategoryID = @ParentCategoryID";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var subCategories = await connection.QueryAsync<Category>(query, new { ParentCategoryID = parentCategoryId });
                return subCategories.ToList();
            }
        }

        public async Task<List<Category>> GetNonParentCategoriesAsync()
        {
            var query = @"SELECT 
                    c.Id,
                    c.CategoryName,
                    c.CategoryDescription,
                    c.ImgPath,
                    c.CreatedOn
                FROM 
                    Category c
                WHERE 
                    c.Id NOT IN (SELECT DISTINCT ParentCategoryID FROM Category WHERE ParentCategoryID IS NOT NULL)";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var nonParentCategories = await connection.QueryAsync<Category>(query);
                return nonParentCategories.ToList();
            }
        }

        public override async Task<Category> GetAsync(int id)
        {
            var query = @"
                        SELECT c.Id,c.CategoryName,c.CategoryDescription,c.ImgPath,c.CreatedOn,c.ParentCategoryID,pc.CategoryName AS ParentCategoryName
                        FROM Category c LEFT JOIN Category pc ON c.ParentCategoryID = pc.Id
                        WHERE c.Id = @Id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var category = await connection.QuerySingleOrDefaultAsync<Category>(query, new { Id = id });
                return category ?? new Category();
            }
        }

        public override async Task<List<Category>> SearchAsync(string search)
        {
            string query = @"SELECT Id,CategoryName,CategoryDescription,ImgPath,CreatedOn 
                          FROM Category WHERE CategoryName LIKE @search";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var categories = await connection.QueryAsync<Category>(query, new { search = "%" + search + "%" });
                return categories.ToList();
            }
        }
    }
}
