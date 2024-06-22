using Shop_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shop_DataAccess.Repository.IRepository
{
    public interface ISizeRepository : IRepository<Size>
    {
        void Update(Size obj);
        Task<bool> AnyAsync(Expression<Func<Size, bool>> predicate);

        Task<List<Size>> ToListAsync();
        IQueryable<Size> GetAllSizes();
        Size GetById(int sizeId);
        Task<IEnumerable<Size>> GetAllAsync();
    }
}
