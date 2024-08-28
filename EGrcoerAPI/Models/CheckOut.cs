using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EGrcoerAPI.Models
{
    public class CheckOut
    {
        public DateTime OrderDeliveryDate { get; set; }
        public string? PaymentMethod { get; set; }

        [Required]
        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        [Required]
        public string? ZipCode { get; set; }
    }
}
