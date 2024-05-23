using Microsoft.EntityFrameworkCore;
using Shop_DataAccess.Repository.IRepository;
using Shop_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shop_DataAccess.Repository
{
    public class ProductSizeRepository : Repository<ProductSize>, IProductSizeRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductSizeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public int GetAvailableQuantity(int productId, int sizeId)
        {
            return _db.ProductSizes
                   .Where(ps => ps.ProductId == productId && ps.SizeId == sizeId)
                   .Select(ps => ps.AvailableQuantity)
                   .FirstOrDefault(); ;
        }
    }
}
