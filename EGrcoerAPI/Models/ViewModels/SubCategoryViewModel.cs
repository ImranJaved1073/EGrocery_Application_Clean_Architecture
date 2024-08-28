namespace EGrcoerAPI.Models
{
    public class SubCategoryViewModel
    {
        public Category? Category { get; set; }
        public List<Category>? SubCategories { get; set; }

        public List<Product>? Products { get; set; }

        
    }
}
