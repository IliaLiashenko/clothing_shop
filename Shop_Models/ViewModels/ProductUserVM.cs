namespace Shop_Models.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM() 
        {
            ProductList = new List<Product>();
            InquiryHeader = new InquiryHeader();
        }

        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
        public IList<Product> ProductList { get; set; }
        public InquiryHeader InquiryHeader { get; set; }


		public Dictionary<int, Dictionary<int, int>> AvailableQuantities { get; set; }
	}
}
