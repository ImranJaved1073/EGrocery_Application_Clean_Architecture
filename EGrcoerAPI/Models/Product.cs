using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EGrcoerAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        [ForeignKey("Brand")]
        public int BrandID { get; set; }

        [ForeignKey("Unit")]
        public int UnitID { get; set; }

        [Required]
        public string? Description { get; set; }

        public string? ProductCode { get; set; }

        public decimal Weight { get; set; }

        [Required]
        public decimal SalePrice { get; set; }
        [Required]
        public decimal Price { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Required]
        public int Quantity { get; set; }

        public string? ImagePath { get; set; }
    }

}
