using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.AccessControl;

namespace EGrcoerAPI.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CategoryName { get; set; }


        [ForeignKey("Id")]
        public int? ParentCategoryID { get; set; }

        [Required]
        public string CategoryDescription { get; set; }

        public string? ImgPath { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
