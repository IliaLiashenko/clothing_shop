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
        public Size Size { get; set; }
        public IEnumerable<SelectListItem> SizeSelectList { get; set; }

        public Dictionary<int, int> AvailableQuantities { get; set; }

        public List<PhotoGallery> GalleryImages { get; set; }

        public ShoppingCart ShoppingCart { get; set; }

    }
}
