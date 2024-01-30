using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    }
}
