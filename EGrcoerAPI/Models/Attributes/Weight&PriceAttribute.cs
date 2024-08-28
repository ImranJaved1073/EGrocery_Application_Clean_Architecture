using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Ecommerce.Models.Attributes
{
    public class Weight_PriceAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string str = Convert.ToString(value)!;

            foreach (var item in str)
            {
                if(!(item > '0' && item<'9'))
                {
                    return new ValidationResult($"Please enter a number.");
                }
            }

            return ValidationResult.Success!;

            
        }
    }
}
