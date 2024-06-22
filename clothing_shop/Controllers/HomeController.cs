using Shop_DataAccess;
using Shop_Models;
using Shop_Models.ViewModels;
using Shop_Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Shop_DataAccess.Repository.IRepository;
using Microsoft.CodeAnalysis;
using Shop_DataAccess.Repository;
using dotless.Core.Parser.Infrastructure;
using NLog.Filters;

namespace clothing_shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _prodRepo;
        private readonly ICategoryRepository _catRepo;
        private readonly IGenderRepository _genRepo;
        private readonly ISizeRepository _sizeRepo;
        private readonly IProductSizeRepository _prodSizeRepo;
        private readonly IColorRepository _colorRepo;
        private readonly IBrandRepository _brandRepo;
        private readonly IStyleRepository _styleRepo;

        public HomeController(ILogger<HomeController> logger, IProductRepository prodRepo, 
            ICategoryRepository catRepo, IGenderRepository genRepo, IColorRepository colorRepo,
            ISizeRepository sizeRepo, IBrandRepository brendRepo, IStyleRepository styleRepo)
        {
            _logger = logger;
            _prodRepo = prodRepo;
            _catRepo = catRepo;
            _genRepo = genRepo;
            _colorRepo = colorRepo;
            _sizeRepo = sizeRepo;
            _brandRepo = brendRepo; 
            _styleRepo = styleRepo;
        }


        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _prodRepo.GetAll(),
                Genders = _genRepo.GetAll()
            };
            return View(homeVM);
        }

        public async Task<IActionResult> Shop(string gender, FilterVM filters)
        {

            if (!string.IsNullOrEmpty(gender))
            {
                var genderEntity = _genRepo.FirstOrDefault(g => g.Name == gender);
                if (genderEntity != null)
                {
                    filters.SelectedGender = genderEntity.Id;
                }
            }

            if (Request.Query["resetFilters"].Count > 0)
            {
                filters = new FilterVM();
            }

            var products = await _prodRepo.GetAllAsync(includeProperties: "Category,Gender,Style,Colors,Brand,ProductSizes");

            if (filters.SelectedGender.HasValue)
                products = products.Where(p => p.GenderId == filters.SelectedGender.Value || p.Gender.Name == "Unisex");
            if (filters.SelectedStyle.HasValue)
                products = products.Where(p => p.StyleId == filters.SelectedStyle.Value);
            if (filters.SelectedCategory.HasValue)
                products = products.Where(p => p.CategoryId == filters.SelectedCategory.Value);
            if (filters.SelectedColor.HasValue)
                products = products.Where(p => p.ColorsId == filters.SelectedColor.Value);
            if (filters.SelectedBrand.HasValue)
                products = products.Where(p => p.BrandId == filters.SelectedBrand.Value);
            if (filters.SelectedSize.HasValue)
            {
                var sizeId = filters.SelectedSize.Value;
                products = products.Where(p => p.ProductSizes.Any(ps => ps.SizeId == sizeId));
            }

            var actualMaxPrice = products.Any() ? products.Max(p => p.Price) : 0;
            var actualMinPrice = products.Any() ? products.Min(p => p.Price) : 0;

            if (!filters.MinPrice.HasValue)
                filters.MinPrice = 0;

            if (filters.MinPrice.HasValue && filters.MinPrice.Value < 0)
                filters.MinPrice = 0;
            if (filters.MaxPrice.HasValue && filters.MaxPrice.Value < actualMinPrice)
                filters.MaxPrice = actualMinPrice;

            if (filters.MinPrice.HasValue && filters.MinPrice.Value > actualMaxPrice)
                filters.MinPrice = actualMaxPrice;
            if (filters.MaxPrice.HasValue && filters.MaxPrice.Value > actualMaxPrice)
                filters.MaxPrice = actualMaxPrice;

            if (filters.MinPrice.HasValue)
                products = products.Where(p => p.Price >= filters.MinPrice.Value);
            if (filters.MaxPrice.HasValue)
                products = products.Where(p => p.Price <= filters.MaxPrice.Value);

            if (!filters.MaxPrice.HasValue || filters.MaxPrice == 0)
            {
                filters.MaxPrice = actualMaxPrice;
            }


            switch (filters.SortOrder)
            {
                case "az":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "za":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "min-max":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "max-min":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "newest":
                    products = products.OrderByDescending(p => p.Id);
                    break;
                default:
                    break;
            }

            var homeVM = new HomeVM
            {
                Products = products.ToList(),
                Categories = await _catRepo.GetAllAsync(),
                Genders = await _genRepo.GetAllAsync(),
                Styles = await _styleRepo.GetAllAsync(),
                Colors = await _colorRepo.GetAllAsync(),
                Brands = await _brandRepo.GetAllAsync(),
                Sizes = await _sizeRepo.GetAllAsync(),
                Filters = filters
            };

            return View(homeVM);
        }

        public IActionResult Details(int Id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            var sizes = _prodRepo.GetProductSizesDropdownList(Id);
            var availableQuantities = _prodRepo.GetAvailableQuantitiesForProduct(Id);
            var galleryImages = _prodRepo.GetGalleryImagesForProduct(Id);

            DetailsVM DetailsVM = new DetailsVM()
            {
                Product = _prodRepo.FirstOrDefault(u => u.Id == Id, includeProperties: "Category"),
                ExistsInCart = shoppingCartList.Any(item => item.ProductId == Id),
                SizeSelectList = new SelectList(sizes, "Value", "Text"),
                AvailableQuantities = availableQuantities,
                GalleryImages = galleryImages
            };

            return View(DetailsVM);
        }


        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id, DetailsVM detailsVM)
        {

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);

            }

            var availableQty = _prodRepo.GetAvailableQuantitiesForProduct(id)
        .GetValueOrDefault(detailsVM.Size.Id, 0);

            if (detailsVM.Product.TempQty > availableQty)
            {
                detailsVM.Product.TempQty = availableQty;
            }

            var existingItem = shoppingCartList.FirstOrDefault(item => item.ProductId == id && item.SizeId == detailsVM.Size.Id);

            if (existingItem != null)
            {
                var newQty = existingItem.Qty + detailsVM.Product.TempQty;
                if (newQty > availableQty)
                {
                    existingItem.Qty = availableQty;
                }
                else
                {
                    existingItem.Qty = newQty;
                }
            }
            else
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = id, SizeId = detailsVM.Size.Id, Qty = detailsVM.Product.TempQty });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Shop));
        }


        public IActionResult RemoveFromCart(int Id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(r => r.ProductId == Id);
            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}