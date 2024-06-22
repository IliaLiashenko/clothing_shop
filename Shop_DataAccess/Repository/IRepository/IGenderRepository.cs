using Shop_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shop_DataAccess.Repository.IRepository
{
    public interface IGenderRepository : IRepository<Gender>
    {
        void Update(Gender obj);
        Task<bool> AnyAsync(Expression<Func<Gender, bool>> predicate);
        Task<IEnumerable<Gender>> GetAllAsync();
    }
}
