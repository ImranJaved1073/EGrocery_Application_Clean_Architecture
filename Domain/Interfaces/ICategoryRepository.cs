

namespace Domain
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<List<Category>> GetNamesAsync();

        Task<List<Category>> GetParentsAsync();

        Task<List<Category>> GetCategoriesWithSubCategoriesAsync();

        Task<List<Category>> GetSubCategoriesAsync(int parentCategoryId);

        Task<List<Category>> GetNonParentCategoriesAsync();

        // Task<Category> GetAsync(int id);

        // Task<List<Category>> SearchAsync(string search);
    }
}
