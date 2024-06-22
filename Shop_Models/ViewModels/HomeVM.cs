using NLog.Filters;
using System.Drawing;
using System.Collections.Generic;
using Shop_Models.ViewModels;
using Shop_Utility;

namespace Shop_Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Gender> Genders { get; set; }
        public IEnumerable<Style> Styles { get; set; }
        public IEnumerable<Colors> Colors { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<Size> Sizes { get; set; }
        public FilterVM Filters { get; set; }

    }
}
