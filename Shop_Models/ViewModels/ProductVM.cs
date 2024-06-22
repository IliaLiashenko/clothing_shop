using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Shop_Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ColorsSelectList { get; set; }
        [ValidateNever]

        public IEnumerable<SelectListItem> SizeSelectList { get; set; }
        [ValidateNever]
        public List<int> SelectedSizeIds { get; set; }
        [ValidateNever]
        public Dictionary<int, int> AvailableQuantities { get; set; }
        [ValidateNever]
        public IEnumerable<PhotoGallery> GalleryPhotos { get; set; }
        [ValidateNever]
        public List<IFormFile> GalleryFiles { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> GenderSelectList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> BrandSelectList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> StyleSelectList { get; set; }
    }
}
