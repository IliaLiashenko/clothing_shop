using Shop_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Html;

namespace Shop_DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);
        IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetAllDropdownList(string obj);
        Task<IEnumerable<Product>> GetAllAsync(Expression<Func<Product, bool>> filter = null,
                                           Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null,
                                           string includeProperties = "");

        Task<Product> GetByIdAsync(int id);
        void AddProductSize(ProductSize productSize);
        List<int> GetSizesByProductId(int productId);
        ProductSize GetProductSize(int productId, int sizeId);
        Task<Product> GetProductWithDetailsAsync(int productId);
        void RemoveProductSize(ProductSize productSize);
        bool Any(Expression<Func<Product, bool>> filter);
        List<SelectListItem> GetProductSizesDropdownList(int productId);

        Dictionary<int, int> GetAvailableQuantitiesForProduct(int productId);

        Dictionary<int, int> GetAvailableQuantitiesForProductAndSize(int productId, int sizeId);

	}
}
