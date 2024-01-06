using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using clothing_shop.Data;
using clothing_shop.Models;
using clothing_shop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace clothing_shop.Controllers
{
    //[Authorize(Roles = WC.AdminRole)]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> objList = _context.Product.Include(u => u.Category);
            return View(objList);
            
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _context.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                ColorsSelectList = _context.Colors.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                SizeSelectList = _context.Size.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                }),
                AvailableQuantities = new Dictionary<int, int>()
            };
            return View(productVM);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                string upload = webRootPath + WC.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                productVM.Product.Image = fileName + extension;

                
                _context.Product.Add(productVM.Product);
                await _context.SaveChangesAsync();

                foreach (var sizeId in productVM.SelectedSizeIds)
                {
                    var productSize = new ProductSize { ProductId = productVM.Product.Id, SizeId = sizeId };

                    if (productVM.AvailableQuantities.ContainsKey(sizeId))
                    {
                        productSize.AvailableQuantity = productVM.AvailableQuantities[sizeId];
                    }

                    _context.ProductSizes.Add(productSize);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            
            productVM.CategorySelectList = _context.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            productVM.ColorsSelectList = _context.Colors.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            productVM.SizeSelectList = _context.Size.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });


            return View(productVM);
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductVM productVM = new ProductVM()
            {
                Product = await _context.Product.FindAsync(id),
                CategorySelectList = _context.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                ColorsSelectList = _context.Colors.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                SizeSelectList = _context.Size.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                }),
                AvailableQuantities = new Dictionary<int, int>(),
                SelectedSizeIds = _context.ProductSizes
            .Where(ps => ps.ProductId == id)
            .Select(ps => ps.SizeId)
            .ToList()
            };
            foreach (var sizeId in productVM.SelectedSizeIds)
            {
                var productSize = _context.ProductSizes
                    .FirstOrDefault(ps => ps.ProductId == id && ps.SizeId == sizeId);

                if (productSize != null)
                {
                    productVM.AvailableQuantities[sizeId] = productSize.AvailableQuantity;
                }
                else
                {
                    productVM.AvailableQuantities[sizeId] = 0; // Или другое значение по умолчанию
                }
            }
            return View(productVM);

        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                var objFromDb = _context.Product.Include(p => p.Colors)
                    .Include(p => p.ProductSizes).FirstOrDefault(u => u.Id == productVM.Product.Id);

                if (objFromDb == null)
                {
                    return NotFound();
                }

                if (files.Count > 0)
                {
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    var oldFile = Path.Combine(upload, objFromDb.Image);

                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.Image = fileName + extension;
                }
                else
                {
                    productVM.Product.Image = objFromDb.Image;
                }
                
                objFromDb.ColorsId = productVM.Product.ColorsId;

                foreach (var sizeId in productVM.SelectedSizeIds)
                {
                    var productSize = objFromDb.ProductSizes.FirstOrDefault(ps => ps.SizeId == sizeId);

                    if (productSize != null)
                    {
                        if (productVM.AvailableQuantities.ContainsKey(sizeId))
                        {
                            productSize.AvailableQuantity = productVM.AvailableQuantities[sizeId];
                        }
                    }
                    else
                    {
                        objFromDb.ProductSizes.Add(new ProductSize { SizeId = sizeId, AvailableQuantity = productVM.AvailableQuantities[sizeId] });
                    }
                }

                foreach (var sizeId in objFromDb.ProductSizes.Select(ps => ps.SizeId).ToList())
                {
                    if (!productVM.SelectedSizeIds.Contains(sizeId))
                    {
                        var productSizeToDelete = objFromDb.ProductSizes.FirstOrDefault(ps => ps.SizeId == sizeId);
                        if (productSizeToDelete != null)
                        {
                            _context.ProductSizes.Remove(productSizeToDelete);
                        }
                    }
                }

                _context.Attach(objFromDb);
                _context.Entry(objFromDb).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            productVM.CategorySelectList = _context.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            productVM.ColorsSelectList = _context.Colors.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            productVM.SizeSelectList = _context.Size.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(productVM);
        }


        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }
            Product product = _context.Product.Include(u=>u.Category).FirstOrDefault(u=>u.Id==id);
            //product.Category = _context.Category.Find(product.CategoryId);

            //var product = await _context.Product
                //.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var obj = _context.Product.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
            

            if (_context.Product == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
