using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain
{
    public class AddProductViewModel
    {
        public Product? Product { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }

        public IEnumerable<SelectListItem>? Brands { get; set; }

        public IEnumerable<SelectListItem>? Units { get; set; }

    }
}
