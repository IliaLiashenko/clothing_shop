using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Shop_Models
{
    public class ProductSize
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int SizeId { get; set; }
        public Size Size { get; set; }
        [ValidateNever]
        public int AvailableQuantity { get; set; }
    }
}
