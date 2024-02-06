using Shop_DataAccess;
using Shop_Models;
using Shop_Models.ViewModels;
using Shop_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Shop_DataAccess.Repository.IRepository;
using System.Text;

namespace clothing_shop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IProductRepository _prodRepo;
        private readonly ISizeRepository _sizeRepo;
        private readonly IInquiryHeaderRepository _inqHRepo;
        private readonly IInquiryDetailRepository _inqDRepo;
        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }
        public CartController(IWebHostEnvironment webHostEnvironment, IApplicationUserRepository userRepo, 
            IProductRepository prodRepo, ISizeRepository sizeRepo,
            IInquiryHeaderRepository inqHRepo, IInquiryDetailRepository inqDRepo)
        {
            _webHostEnvironment = webHostEnvironment;
            _userRepo = userRepo;
            _prodRepo = prodRepo;
            _sizeRepo = sizeRepo;
            _inqHRepo = inqHRepo;
            _inqDRepo = inqDRepo;
        }

        public IActionResult Index()
        {

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodListTemp = _prodRepo.GetAll(u => prodInCart.Contains(u.Id));
            IList<Product> prodList = new List<Product>();

			foreach (var cartObj in shoppingCartList)
            {
                Product prodTemp = prodListTemp.FirstOrDefault(u => u.Id == cartObj.ProductId);

                prodTemp.TempQty = cartObj.Qty;
                prodTemp.Size = _sizeRepo.GetById(cartObj.SizeId);
                prodList.Add(prodTemp);
			}

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> ProdList)
        {
			List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
			foreach (Product prod in ProdList)
			{
				shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, Qty = prod.TempQty, SizeId = prod.Size.Id });
			}

			HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
			return RedirectToAction(nameof(Summary));
        }


        public IActionResult Summary()
        {
			ApplicationUser applicationUser;
            if (User.IsInRole(WC.AdminRole))
            {
				if (HttpContext.Session.Get<int>(WC.SessionInquiryId) != 0)
				{
					InquiryHeader inquiryHeader = _inqHRepo.FirstOrDefault(u => u.Id == HttpContext.Session.Get<int>(WC.SessionInquiryId));
					applicationUser = new ApplicationUser()
					{
						Email = inquiryHeader.Email,
						FullName = inquiryHeader.FullName,
						PhoneNumber = inquiryHeader.PhoneNumber,
                        StreetAddress = inquiryHeader.StreetAddress,
                        City = inquiryHeader.City,
                        State = inquiryHeader.State,
                        PostalCode = inquiryHeader.PostalCode
					};
				}
                else
                {
                    applicationUser = new ApplicationUser();
                }
			}
            else
            {
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
				//var userId = User.FindFirstValue(ClaimTypes.Name);
				applicationUser = _userRepo.FirstOrDefault(u => u.Id == claim.Value);
			}


            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _prodRepo.GetAll(u => prodInCart.Contains(u.Id));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = applicationUser
            };

            foreach(var cartObj in shoppingCartList)
            {
				Product prodTemp = _prodRepo.FirstOrDefault(u => u.Id == cartObj.ProductId);
				prodTemp.TempQty = cartObj.Qty;
				prodTemp.Size = _sizeRepo.GetById(cartObj.SizeId);

                ProductUserVM.ProductList.Add(prodTemp);
			}

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM ProductUserVM)
        {
            
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";
            var subject = "New Inquiry";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            //Name: {0}
            //Email: {1}
            //Phone: {2}
            //Products: {3}

            StringBuilder productListSB = new StringBuilder();
            foreach (var prod in ProductUserVM.ProductList)
            {
                productListSB.Append($" - Name: {prod.ProductName} <span style='font-size:14px;'> (ID: {prod.Id})</span><br />");
            }

            string messageBody = string.Format(HtmlBody,
                    ProductUserVM.ApplicationUser.FullName,
                    ProductUserVM.ApplicationUser.Email,
                    ProductUserVM.ApplicationUser.PhoneNumber,
                    ProductUserVM.ApplicationUser.StreetAddress,
                    ProductUserVM.ApplicationUser.City,
                    ProductUserVM.ApplicationUser.State,
                    ProductUserVM.ApplicationUser.PostalCode,
                    productListSB.ToString());


            //await _emailSender...

            InquiryHeader inquiryHeader = new InquiryHeader()
            {
                ApplicationUserId = claim.Value,
                FullName = ProductUserVM.ApplicationUser.FullName,
                Email = ProductUserVM.ApplicationUser.Email,
                PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                StreetAddress = ProductUserVM.ApplicationUser.StreetAddress,
                City = ProductUserVM.ApplicationUser.City,
                State = ProductUserVM.ApplicationUser.State,
                PostalCode = ProductUserVM.ApplicationUser.PostalCode,
                InquiryDate = DateTime.Now
            };

            _inqHRepo.Add(inquiryHeader);
            await _inqHRepo.SaveAsync();
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            foreach (var prod in shoppingCartList)
            {
                InquiryDetail inquiryDetail = new InquiryDetail()
                {
                    InquiryHeaderId = inquiryHeader.Id,
                    ProductId = prod.ProductId,
                    SizeId = prod.SizeId,
                    Qty = prod.Qty

                };
                _inqDRepo.Add(inquiryDetail);
                await _inqDRepo.SaveAsync();
            }

            return RedirectToAction(nameof(InquiryConfirmation));
        }
        public IActionResult InquiryConfirmation(ProductUserVM ProductUserVM)
        {
            HttpContext.Session.Clear();

            return View();
        }

        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart)!=null 
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            var itemToRemove = shoppingCartList.FirstOrDefault(u => u.ProductId == id);
            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		public IActionResult UpdateCart(IEnumerable<Product> ProdList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product prod in ProdList)
            {
				shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, Qty = prod.TempQty, SizeId = prod.Size.Id});
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

	}
}
