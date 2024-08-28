namespace EGrcoerAPI.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }

        public decimal Weight { get; set; }

        public string? Unit { get; set; }
    }
}
