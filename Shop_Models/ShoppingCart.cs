using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Qty { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

    }
}
