
namespace Domain
{
    public interface ICategoryRepository : IRepository<Category>
    {
        public List<Category> GetNames();

        public List<Category> GetParents();

        public List<Category> GetCategoriesWithSubCategories();

        public List<Category> GetSubCategories(int parentCategoryId);

        public List<Category> GetNonParentCategories();

        //public Category Get(int id);

        //public List<Category> Search(string search);
    }
}
