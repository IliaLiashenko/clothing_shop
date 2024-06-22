using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop_Models.ViewModels
{
    public class FilterVM
    {
        public int? SelectedGender { get; set; }
        public int? SelectedStyle { get; set; }
        public int? SelectedCategory { get; set; }
        public int? SelectedColor { get; set; }
        public int? SelectedBrand { get; set; }
        public int? SelectedSize { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }

        public string SortOrder { get; set; }
    }
}
