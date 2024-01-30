using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Shop_Models.ViewModels
{
    public class DetailsVM
    {
        public DetailsVM() 
        { 
            Product = new Product();
        }
        public Product Product { get; set; }
        public bool ExistsInCart { get; set; }

        [Display(Name = "Size")]
        public int SelectedSizeId { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        public IEnumerable<SelectListItem> SizeSelectList { get; set; }
    }
}
