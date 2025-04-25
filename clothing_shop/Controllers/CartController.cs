using Shop_DataAccess;
using Shop_Models;
using Shop_Models.ViewModels;
using Shop_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Shop_DataAccess.Repository.IRepository;
using System.Text;
using Stripe.Checkout;
//using Shop_Utility.BrainTree;

namespace clothing_shop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IProductRepository _prodRepo;
        private readonly ISizeRepository _sizeRepo;
        private readonly IProductSizeRepository _prodSizeRepo;
        private readonly IInquiryHeaderRepository _inqHRepo;
        private readonly IInquiryDetailRepository _inqDRepo;
        private readonly IShoppingCartRepository _cartRepo;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }
        public CartController(IWebHostEnvironment webHostEnvironment, IApplicationUserRepository userRepo, 
            IProductRepository prodRepo, ISizeRepository sizeRepo, IProductSizeRepository prodSizeRepo,
            IInquiryHeaderRepository inqHRepo, IInquiryDetailRepository inqDRepo, IShoppingCartRepository cartRepo)
        {
            _webHostEnvironment = webHostEnvironment;
            _userRepo = userRepo;
            _prodRepo = prodRepo;
            _sizeRepo = sizeRepo;
            _prodSizeRepo = prodSizeRepo;
            _inqHRepo = inqHRepo;
            _inqDRepo = inqDRepo;
            _cartRepo = cartRepo;
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

            ProductUserVM productUserVM = new ProductUserVM
            {
                ProductList = new List<Product>(),
                AvailableQuantities = new Dictionary<int, Dictionary<int, int>>()
            };

            foreach (var cartObj in shoppingCartList)
            {
                Product prodTemp = CloneProduct(prodListTemp.FirstOrDefault(u => u.Id == cartObj.ProductId));

                prodTemp.Size = _sizeRepo.GetById(cartObj.SizeId);
                prodList.Add(prodTemp);

                int availableQty = _prodRepo.GetAvailableQuantitiesForProductAndSize(cartObj.ProductId, cartObj.SizeId)[cartObj.SizeId];
                if (!productUserVM.AvailableQuantities.ContainsKey(cartObj.ProductId))
                {
                    productUserVM.AvailableQuantities[cartObj.ProductId] = new Dictionary<int, int>();
                }
                productUserVM.AvailableQuantities[cartObj.ProductId][cartObj.SizeId] = availableQty;
            }

            productUserVM.ProductList = prodList;
            productUserVM.ShoppingCartList = shoppingCartList;

            return View(productUserVM);
        }

        private Product CloneProduct(Product original)
        {
            return new Product
            {
                Id = original.Id,
                ProductName = original.ProductName,
                ProductDescription = original.ProductDescription,
                Price = original.Price,
                ColorsId = original.ColorsId,
                Image = original.Image,
                DisplayOrder = original.DisplayOrder,
                CategoryId = original.CategoryId
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(ProductUserVM productUserVM)
        {
            if (productUserVM.ShoppingCartList != null && productUserVM.ShoppingCartList.Any())
            {
                HttpContext.Session.Set(WC.SessionCart, productUserVM.ShoppingCartList);
            }

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
                ApplicationUser = applicationUser,
                ProductList = new List<Product>(),
                ShoppingCartList = new List<ShoppingCart>()
            };

            foreach(var cartObj in shoppingCartList)
            {
                Product prodTemp = _prodRepo.FirstOrDefault(u => u.Id == cartObj.ProductId);
                Size sizeTemp = _sizeRepo.GetById(cartObj.SizeId);

                prodTemp.Size = sizeTemp;

                ProductUserVM.ProductList.Add(prodTemp);
                ProductUserVM.ShoppingCartList.Add(new ShoppingCart
                {
                    ProductId = cartObj.ProductId,
                    SizeId = cartObj.SizeId,
                    Qty = cartObj.Qty
                });

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

            //ApplicationUser applicationUser = _userRepo.FirstOrDefault(u => u.Id == claim.Value);
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            //Name: {0}
            //Email: {1}
            //Phone: {2}
            //Products: {3}

            ProductUserVM.InquiryHeader.OrderTotal = 0;

            foreach (var cartItem in ProductUserVM.ShoppingCartList)
            {
                var product = ProductUserVM.ProductList
                    .FirstOrDefault(p => p.Id == cartItem.ProductId && p.Size?.Id == cartItem.SizeId);

                if (product != null)
                {
                    ProductUserVM.InquiryHeader.OrderTotal += product.Price * cartItem.Qty;
                }
            }



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
                InquiryDate = System.DateTime.Now,
                OrderTotal = ProductUserVM.InquiryHeader.OrderTotal,
                PaymentStatus = WC.PaymentStatusPending,
                OrderStatus = WC.StatusPending
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


            var domain = $"{Request.Scheme}://{Request.Host}/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"Cart/InquiryConfirmation?id={inquiryHeader.Id}",
                CancelUrl = domain + "Cart/index",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in ProductUserVM.ProductList)
            {
                var cartItem = ProductUserVM.ShoppingCartList.FirstOrDefault(c =>
                    c.ProductId == item.Id && c.SizeId == item.Size?.Id);

                if (cartItem != null)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // Example: 20.50 => 2050
                            Currency = "eur",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"{item.ProductName} (Size: {item.Size?.Name})"
                            }
                        },
                        Quantity = cartItem.Qty
                    };

                    options.LineItems.Add(sessionLineItem);
                }
            }


            var service = new Stripe.Checkout.SessionService();
            Session session = service.Create(options);
            _inqHRepo.UpdateStripePaymentID(inquiryHeader.Id, session.Id, session.PaymentIntentId);
            _inqHRepo.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);


            //return RedirectToAction(nameof(InquiryConfirmation));
        }
        public IActionResult InquiryConfirmation(/*ProductUserVM ProductUserVM*/ int id)
        {
            //HttpContext.Session.Clear();
            InquiryHeader inquiryHeader = _inqHRepo.FirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser", isTracking: false);
            if(inquiryHeader.PaymentStatus != WC.PaymentStatusDelayedPayment) 
            { 
                var service = new SessionService();
                Session session = service.Get(inquiryHeader.SessionId);

                if(session.PaymentStatus.ToLower() == "paid")
                {
                    _inqHRepo.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _inqHRepo.UpdateStatus(id, WC.StatusApproved, WC.PaymentStatusApproved);
                    _inqHRepo.Save();

                    List<InquiryDetail> inquiryDetails = _inqDRepo.GetAll(u => u.InquiryHeaderId == id).ToList();
                    foreach (var detail in inquiryDetails)
                    {
                        ProductSize productSize = _prodSizeRepo.FirstOrDefault(
                    u => u.ProductId == detail.ProductId && u.SizeId == detail.SizeId);

                        if (productSize != null)
                        {
                            productSize.AvailableQuantity -= detail.Qty;
                            _prodSizeRepo.Update(productSize);
                        }
                    }
                    _prodSizeRepo.Save();
                }

                HttpContext.Session.Clear();
            }

            List<ShoppingCart> shoppingCarts = _cartRepo
                .GetAll(u=>u.ApplicationUserId == inquiryHeader.ApplicationUserId).ToList();

            _cartRepo.RemoveRange(shoppingCarts);
            _cartRepo.Save();

            return View(id);
        }

        public IActionResult Remove(int productId, int sizeId)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart)!=null 
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            var itemToRemove = shoppingCartList.FirstOrDefault(u => u.ProductId == productId && u.SizeId == sizeId);
            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(List<ShoppingCart> cartUpdates)
        {
            if (cartUpdates == null || !cartUpdates.Any())
            {
                return Json(new { success = false, message = "Empty cart update." });
            }

            // Обновляем сессию новой версией корзины
            HttpContext.Session.Set(WC.SessionCart, cartUpdates);

            return Json(new { success = true });
        }



    }
}
