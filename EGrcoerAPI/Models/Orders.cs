using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EGrcoerAPI.Models
{
    public class Orders
    {
        public int Id { get; set; }
        public string? UserId { get; set; }

        [NotMapped]
        public IdentityUser? User { get; set; }
        public string? OrderNum { get; set; }
        public DateTime OrderDate { get; set; }

        public string? Status { get; set; }

        public decimal TotalBill { get; set; }
        //public decimal TotalDiscount { get; set; }
        [NotMapped]
        public CheckOut? CheckOut { get; set; }
        public DateTime OrderDeliveryDate { get; set; }
        public string? PaymentMethod { get; set; }

        [Required]
        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        [Required]
        public string? ZipCode { get; set; }
        //public string PaymentStatus { get; set; }

        [NotMapped]
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
