﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop_DataAccess.Repository.IRepository;
using Shop_Models;
using Shop_Models.ViewModels;
using Shop_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Html;

namespace Shop_DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ApplicationDbContext db, ILogger<ProductRepository> logger) : base(db)
        {
            _db = db;
            _logger = logger;

        }

        public async Task<IEnumerable<Product>> GetAllAsync(Expression<Func<Product, bool>>? filter = null,
                                                   Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
                                                   string includeProperties = "")
        {
            IQueryable<Product> query = _db.Product;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            query = IncludeProperties(query, includeProperties);
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return await query.ToListAsync().ConfigureAwait(false);
        }
        private IQueryable<Product> IncludeProperties(IQueryable<Product> query, string includeProperties)
        {
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            return query;
        }
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetAllDropdownList(string obj)
        {
            if(obj==WC.CategoryName)
            {
                return _db.Category.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            if(obj==WC.ColorsName)
            {
                return _db.Colors.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            if (obj == WC.SizeName)
            {
                return _db.Size.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                });
            }
            return null;
            //return Enumerable.Empty<SelectListItem>();
        }

        public void AddProductSize(ProductSize productSize)
        {
            _db.ProductSizes.Add(productSize);
            _db.SaveChanges();
        }
        public List<int> GetSizesByProductId(int productId)
        {
            return _db.ProductSizes
                .Where(ps => ps.ProductId == productId)
                .Select(ps => ps.SizeId)
                .ToList();
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            return await _db.Product.FindAsync(id);
        }
        public ProductSize GetProductSize(int productId, int sizeId)
        {
            return _db.ProductSizes.FirstOrDefault(ps => ps.ProductId == productId && ps.SizeId == sizeId);
        }
        public async Task<Product> GetProductWithDetailsAsync(int productId)
        {
            return await _db.Product
                .AsNoTracking()
                .Include(p => p.Colors)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(u => u.Id == productId);
        }

        public void RemoveProductSize(ProductSize productSize)
        {
            _db.ProductSizes.Remove(productSize);
            _db.SaveChanges();
        }

        public void Update(Product obj)
        {
            _db.Update(obj);  
        }

        public bool Any(Expression<Func<Product, bool>> filter)
        {
            return _db.Product.Any(filter);
        }

        public List<SelectListItem> GetProductSizesDropdownList(int productId)
        {
            return _db.ProductSizes
                .Where(ps => ps.ProductId == productId)
                .Select(ps => new SelectListItem { Value = ps.SizeId.ToString(), Text = ps.Size.Name })
                .ToList();
        }
    }
}
