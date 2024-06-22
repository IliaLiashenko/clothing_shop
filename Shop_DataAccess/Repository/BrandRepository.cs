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
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private readonly ApplicationDbContext _db;
        public BrandRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public async Task<bool> AnyAsync(Expression<Func<Brand, bool>> predicate)
        {
            return await dbSet.AnyAsync(predicate);
        }
        public void Update(Brand obj)
        {
            var objFromDb = base.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
            }
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            return await _db.Brands.ToListAsync();
        }
    }
}
