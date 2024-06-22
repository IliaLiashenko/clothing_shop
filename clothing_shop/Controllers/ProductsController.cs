using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop_DataAccess;
using Shop_Models;
using Shop_Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Shop_DataAccess.Repository.IRepository;
using Shop_Utility;
using Microsoft.CodeAnalysis;
using dotless.Core.Parser.Infrastructure;
using Shop_DataAccess.Repository;

namespace clothing_shop.Controllers
{
    [Authorize(Roles = Shop_Utility.WC.AdminRole)]
    public class ProductsController : Controller
    {
        private readonly IProductRepository _prodRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IProductRepository prodRepo, IWebHostEnvironment webHostEnvironment)
        {
            _prodRepo = prodRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _prodRepo.GetAll(includeProperties: "Category");
            return View(objList);

        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _prodRepo == null)
            {
                return NotFound();
            }

            var product = await _prodRepo.GetAllAsync(filter: m => m.Id == id,
            includeProperties: "Category,Colors,ProductSizes.Size");
            if (product == null)
            {
                return NotFound();
            }

            return View(product.FirstOrDefault());
        }

        // GET: Products/Create
        public IActionResult Create(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _prodRepo.GetAllDropdownList(WC.CategoryName),
                ColorsSelectList = _prodRepo.GetAllDropdownList(WC.ColorsName),
                SizeSelectList = _prodRepo.GetAllDropdownList(WC.SizeName),
                GenderSelectList = _prodRepo.GetAllDropdownList(WC.GenderName),
                BrandSelectList = _prodRepo.GetAllDropdownList(WC.BrandName),
                StyleSelectList = _prodRepo.GetAllDropdownList(WC.StyleName),
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

                string upload = webRootPath + Shop_Utility.WC.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                var mainImageFile = files.FirstOrDefault(f => f.Name == "ProductImage");
                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    mainImageFile.CopyTo(fileStream);
                }

                productVM.Product.Image = fileName + extension;

                _prodRepo.Add(productVM.Product);
                await _prodRepo.SaveAsync();

                string galleryUploadPath = webRootPath + WC.GalleryPath;
                foreach (var file in productVM.GalleryFiles)
                {
                    if (file != null && file.Length > 0)
                    {
                        string galleryFileName = Guid.NewGuid().ToString();
                        string galleryExtension = Path.GetExtension(file.FileName);
                        string galleryFilePath = Path.Combine(galleryUploadPath, galleryFileName + galleryExtension);

                        using (var fileStream = new FileStream(galleryFilePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        var photoGallery = new PhotoGallery
                        {
                            Url = Shop_Utility.WC.GalleryPath + "/" + galleryFileName + galleryExtension,
                            ProductId = productVM.Product.Id
                        };

                        _prodRepo.AddPhotoToGallery(photoGallery);
                    }

                }


                await _prodRepo.SaveAsync();

                foreach (var sizeId in productVM.SelectedSizeIds)
                {
                    var productSize = new ProductSize { ProductId = productVM.Product.Id, SizeId = sizeId };

                    if (productVM.AvailableQuantities.ContainsKey(sizeId))
                    {
                        productSize.AvailableQuantity = productVM.AvailableQuantities[sizeId];
                    }

                    _prodRepo.AddProductSize(productSize);
                }
                await _prodRepo.SaveAsync();

                return RedirectToAction("Index");
            }


            productVM.CategorySelectList = _prodRepo.GetAllDropdownList(WC.CategoryName);
            productVM.ColorsSelectList = _prodRepo.GetAllDropdownList(WC.ColorsName);
            productVM.SizeSelectList = _prodRepo.GetAllDropdownList(WC.SizeName);
            productVM.GenderSelectList = _prodRepo.GetAllDropdownList(WC.GenderName);
            productVM.BrandSelectList = _prodRepo.GetAllDropdownList(WC.BrandName);
            productVM.StyleSelectList = _prodRepo.GetAllDropdownList(WC.StyleName);


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
                Product = await _prodRepo.GetByIdAsync(id.GetValueOrDefault()),
                CategorySelectList = _prodRepo.GetAllDropdownList(WC.CategoryName),
                ColorsSelectList = _prodRepo.GetAllDropdownList(WC.ColorsName),
                SizeSelectList = _prodRepo.GetAllDropdownList(WC.SizeName),
                GenderSelectList = _prodRepo.GetAllDropdownList(WC.GenderName),
                BrandSelectList = _prodRepo.GetAllDropdownList(WC.BrandName),
                StyleSelectList = _prodRepo.GetAllDropdownList(WC.StyleName),
                AvailableQuantities = new Dictionary<int, int>(),
                SelectedSizeIds = _prodRepo.GetSizesByProductId(id.GetValueOrDefault()),
                GalleryPhotos = await _prodRepo.GetGalleryFiles(id.GetValueOrDefault()) ?? new List<PhotoGallery>()
            };
            foreach (var sizeId in productVM.SizeSelectList.Select(size => int.Parse(size.Value)))
            {
                if (!productVM.AvailableQuantities.ContainsKey(sizeId))
                {
                    productVM.AvailableQuantities[sizeId] = 0;
                }
            }
            foreach (var sizeId in productVM.SelectedSizeIds)
            {
                var productSize = _prodRepo.GetProductSize(id.GetValueOrDefault(), sizeId);

                if (productSize != null)
                {
                    productVM.AvailableQuantities[sizeId] = productSize.AvailableQuantity;
                }
                else
                {
                    productVM.AvailableQuantities[sizeId] = 0;
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
                var objFromDb = _prodRepo.GetProductWithDetailsAsync(productVM.Product.Id).Result;


                if (objFromDb == null)
                {
                    return NotFound();
                }
                var mainImageFile = files.FirstOrDefault(f => f.Name == "ProductImage");
                if (mainImageFile != null)
                {
                    string upload = webRootPath + Shop_Utility.WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    var oldFile = Path.Combine(upload, objFromDb.Image);

                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        mainImageFile.CopyTo(fileStream);
                    }
                    productVM.Product.Image = fileName + extension;
                }
                else
                {
                    productVM.Product.Image = objFromDb.Image;
                }

                objFromDb.ColorsId = productVM.Product.ColorsId;
                objFromDb.ProductName = productVM.Product.ProductName;
                objFromDb.ProductDescription = productVM.Product.ProductDescription;
                objFromDb.Price = productVM.Product.Price;
                objFromDb.Image = productVM.Product.Image;
                objFromDb.DisplayOrder = productVM.Product.DisplayOrder;
                objFromDb.CategoryId = productVM.Product.CategoryId;
                objFromDb.GenderId = productVM.Product.GenderId;
                objFromDb.BrandId = productVM.Product.BrandId;
                objFromDb.StyleId = productVM.Product.StyleId;

                foreach (var sizeId in productVM.SelectedSizeIds)
                {
                    var productSize = objFromDb.ProductSizes.FirstOrDefault(ps => ps.SizeId == sizeId);

                    if (productSize != null)
                    {
                        if (productVM.AvailableQuantities.ContainsKey(sizeId))
                        {
                            productSize.AvailableQuantity = productVM.AvailableQuantities[sizeId];
                        }
                        else
                        {
                            productSize.AvailableQuantity = 0;
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
                            _prodRepo.RemoveProductSize(productSizeToDelete);
                        }
                    }
                }

                if (productVM.GalleryFiles != null && productVM.GalleryFiles.Any())
                {
                    string galleryUploadPath = webRootPath + WC.GalleryPath;
                    foreach (var file in productVM.GalleryFiles)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(file.FileName);

                        using (var fileStream = new FileStream(Path.Combine(galleryUploadPath, fileName + extension), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        var photoGallery = new PhotoGallery
                        {
                            Url = Path.Combine(WC.GalleryPath, fileName + extension),
                            ProductId = productVM.Product.Id
                        };

                        _prodRepo.AddPhotoToGallery(photoGallery);
                    }
                }


                _prodRepo.Update(objFromDb);
                await _prodRepo.SaveAsync();
                return RedirectToAction("Index");
            }

            productVM.CategorySelectList = _prodRepo.GetAllDropdownList(WC.CategoryName);
            productVM.ColorsSelectList = _prodRepo.GetAllDropdownList(WC.ColorsName);
            productVM.SizeSelectList = _prodRepo.GetAllDropdownList(WC.SizeName);
            productVM.GenderSelectList = _prodRepo.GetAllDropdownList(WC.GenderName);
            productVM.BrandSelectList = _prodRepo.GetAllDropdownList(WC.BrandName);
            productVM.StyleSelectList = _prodRepo.GetAllDropdownList(WC.StyleName);

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> UploadGalleryPhotos(int productId, List<IFormFile> galleryFiles)
        {
            if (galleryFiles != null && galleryFiles.Count > 0)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                string galleryPath = webRootPath + WC.GalleryPath;

                foreach (var file in galleryFiles)
                {
                    if (file != null && file.Length > 0)
                    {
                        string galleryFileName = Guid.NewGuid().ToString();
                        string galleryExtension = Path.GetExtension(file.FileName);
                        string galleryFilePath = Path.Combine(galleryPath, galleryFileName + galleryExtension);

                        using (var fileStream = new FileStream(galleryFilePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        var photoGallery = new PhotoGallery
                        {
                            Url = WC.GalleryPath + "/" + galleryFileName + galleryExtension,
                            ProductId = productId
                        };

                        _prodRepo.AddPhotoToGallery(photoGallery);
                    }
                }

                await _prodRepo.SaveAsync();
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGalleryPhoto(int id)
        {
            var photo = await _prodRepo.GetPhotoGalleryByIdAsync(id);
            if (photo == null)
            {
                return Json(new { success = false, message = "Photo not found." });
            }

            var webRootPath = _webHostEnvironment.WebRootPath;
            var filePath = Path.Combine(webRootPath, photo.Url.TrimStart('/'));

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _prodRepo.RemovePhotoGallery(photo);
            await _prodRepo.SaveAsync();

            return Json(new { success = true, message = "Photo deleted successfully." });
        }


        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _prodRepo == null)
            {
                return NotFound();
            }
            Product product = await _prodRepo.GetProductWithDetailsAsync(id.GetValueOrDefault());
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

            var obj = await _prodRepo.GetByIdAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            string upload = _webHostEnvironment.WebRootPath + Shop_Utility.WC.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }


            if (_prodRepo == null)
            {
                return NotFound();
            }
            var product = await _prodRepo.GetByIdAsync(id);
            if (product != null)
            {
                _prodRepo.Remove(product);
            }

            await _prodRepo.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _prodRepo.Any(p => p.Id == id);
        }
    }
}
