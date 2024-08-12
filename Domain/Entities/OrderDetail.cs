using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public Product? Product { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }

        [NotMapped]
        public decimal TotalPrice { get; set; }
    }
}
