using Shop_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shop_DataAccess.Repository.IRepository
{
    public interface IStyleRepository : IRepository<Style>
    {
        void Update(Style obj);
        Task<bool> AnyAsync(Expression<Func<Style, bool>> predicate);
        Task<IEnumerable<Style>> GetAllAsync();
    }
}
