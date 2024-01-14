namespace clothing_shop.Models.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM() 
        {
            ProductList = new List<Product>();
        }

        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<Product> ProductList { get; set; }
        public List<ShoppingCart> ShoppingCartList { get; set; }
    }
}
