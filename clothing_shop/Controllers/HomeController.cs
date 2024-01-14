using clothing_shop.Data;
using clothing_shop.Models;
using clothing_shop.Models.ViewModels;
using clothing_shop.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace clothing_shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }


        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _db.Product.Include(u => u.Category),
                Categories = _db.Category
            };
            return View(homeVM);
        }

        public IActionResult Main()
        {
            
            return View("Main");
        }

        public IActionResult Details(int Id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            var sizes = _db.ProductSizes
                            .Where(ps => ps.ProductId == Id)
                            .Select(ps => new SelectListItem { Value = ps.SizeId.ToString(), Text = ps.Size.Name })
                            .ToList();

            DetailsVM DetailsVM = new DetailsVM()
            {
                Product = _db.Product.Include(u => u.Category).Where(u => u.Id == Id).FirstOrDefault(),
                ExistsInCart = shoppingCartList.Any(item => item.ProductId == Id),
                SizeSelectList = new SelectList(sizes, "Value", "Text")
            };

            return View(DetailsVM);
        }


        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(DetailsVM model)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            // Добавляем выбранный размер и количество
            shoppingCartList.Add(new ShoppingCart { ProductId = model.Product.Id, SizeId = model.SelectedSizeId, Quantity = model.Quantity });

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
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