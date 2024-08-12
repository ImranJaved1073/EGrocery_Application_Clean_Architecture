using Domain.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Drawing.Drawing2D;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Http;

namespace Domain
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
        [Weight_Price()]
        public int Quantity { get; set; }

        [NotMapped]
        public IFormFile? Picture { get; set; }

        public string? ImagePath { get; set; }

        [NotMapped]
        public string?  CategoryName { get; set; }

        [NotMapped]
        public string? BrandName { get; set; }

        [NotMapped]
        public string? UnitName { get; set; }


        //public string GetUnitName(int id)
        //{
        //    IRepository<Unit> unitRepository = new GenericRepository<Unit>(@"Data Source=DESKTOP-EQ55Q8H\SQLEXPRESS;Initial Catalog=GroceryDb;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Encrypt=False;Trust Server Certificate=True;Command Timeout=0");
        //    UnitName = unitRepository.Get(id).Name;
        //    return UnitName!;
        //}
    }

}
