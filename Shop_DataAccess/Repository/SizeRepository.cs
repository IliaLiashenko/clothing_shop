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
    public class SizeRepository : Repository<Size>, ISizeRepository
    {
        private readonly ApplicationDbContext _db;
        public SizeRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public async Task<bool> AnyAsync(Expression<Func<Size, bool>> predicate)
        {
            return await dbSet.AnyAsync(predicate);
        }
        public void Update(Size obj)
        {
            var objFromDb = base.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
            }
        }


        public async Task<List<Size>> ToListAsync()
        {
            return await _db.Size.ToListAsync();
        }
        public IQueryable<Size> GetAllSizes()
        {
            return _db.Size;
        }

        public Size GetById(int sizeId)
        {
            return _db.Size.FirstOrDefault(s => s.Id == sizeId);
        }

    }
}
