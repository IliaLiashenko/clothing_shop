using Shop_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shop_DataAccess.Repository.IRepository
{
    public interface IProductSizeRepository : IRepository<ProductSize>
    {
        int GetAvailableQuantity(int productId, int sizeId);
    }
}
