using Shop_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shop_DataAccess.Repository.IRepository
{
    public interface IBrandRepository : IRepository<Brand>
    {
        void Update(Brand obj);
        Task<bool> AnyAsync(Expression<Func<Brand, bool>> predicate);
        Task<IEnumerable<Brand>> GetAllAsync();
    }
}
