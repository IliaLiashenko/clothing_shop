namespace Shop_Models.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM() 
        {
            ProductList = new List<Product>();
        }

        public ApplicationUser ApplicationUser { get; set; }
        public IList<Product> ProductList { get; set; }
        public List<ShoppingCart> ShoppingCartList { get; set; }
    }
}
