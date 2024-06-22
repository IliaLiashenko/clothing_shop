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
    public class GenderRepository : Repository<Gender>, IGenderRepository
    {
        private readonly ApplicationDbContext _db;
        public GenderRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public async Task<bool> AnyAsync(Expression<Func<Gender, bool>> predicate)
        {
            return await dbSet.AnyAsync(predicate);
        }
        public void Update(Gender obj)
        {
            var objFromDb = base.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
            }
        }

        public async Task<IEnumerable<Gender>> GetAllAsync()
        {
            return await _db.Genders.ToListAsync();
        }
    }
}
