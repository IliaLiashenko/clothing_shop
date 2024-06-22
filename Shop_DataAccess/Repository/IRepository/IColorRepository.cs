using Shop_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shop_DataAccess.Repository.IRepository
{
    public interface IColorRepository : IRepository<Colors>
    {
        void Update(Colors obj);
        Task<bool> AnyAsync(Expression<Func<Colors, bool>> predicate);
        Task<IEnumerable<Colors>> GetAllAsync();
    }
}
