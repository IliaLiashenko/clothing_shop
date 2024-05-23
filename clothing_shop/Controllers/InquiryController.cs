using Shop_DataAccess;
using Shop_Models;
using Shop_Models.ViewModels;
using Shop_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Shop_DataAccess.Repository.IRepository;
using System.Text;
using dotless.Core.Parser.Tree;

namespace clothing_shop.Controllers
{
    [Authorize(Roles=WC.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inqHRepo;
        private readonly IInquiryDetailRepository _inqDRepo;
        [BindProperty]
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(IInquiryHeaderRepository inqHRepo, IInquiryDetailRepository inqDRepo)
        {
            _inqHRepo = inqHRepo;
            _inqDRepo = inqDRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            InquiryVM = new InquiryVM()
            {
                InquiryHeader = _inqHRepo.FirstOrDefault(u => u.Id == id, includeProperties:"ApplicationUser"),
                InquiryDetail = _inqDRepo.GetAll(u => u.InquiryHeaderId == id, includeProperties: "Product,Size")
            };
            return View(InquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            InquiryVM.InquiryDetail = _inqDRepo.GetAll(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);
            foreach (var detail in InquiryVM.InquiryDetail)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId,
                    Qty = 1
                };
                shoppingCartList.Add(shoppingCart);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WC.SessionInquiryId, InquiryVM.InquiryHeader.Id);
            return RedirectToAction("Index", "Cart");
        }


        [HttpPost]
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = _inqHRepo.FirstOrDefault(u => u.Id == InquiryVM.InquiryHeader.Id);
            IEnumerable<InquiryDetail> inquiryDetails = _inqDRepo.GetAll(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);

            _inqDRepo.RemoveRange(inquiryDetails);
            _inqHRepo.Remove(inquiryHeader);
            _inqHRepo.Save();
            
            return RedirectToAction(nameof(Index));
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<InquiryHeader> objInquiryHeaders = _inqHRepo.GetAll(includeProperties: "ApplicationUser").ToList();

            switch (status)
            {
                case "pending":
                    objInquiryHeaders = objInquiryHeaders.Where(u => u.PaymentStatus == WC.PaymentStatusPending);
                    break;
                case "inprocess":
                    objInquiryHeaders = objInquiryHeaders.Where(u => u.OrderStatus == WC.StatusInProcess);
                    break;
                case "completed":
                    objInquiryHeaders = objInquiryHeaders.Where(u => u.OrderStatus == WC.StatusShipped);
                    break;
                case "approved":
                    objInquiryHeaders = objInquiryHeaders.Where(u => u.OrderStatus == WC.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = objInquiryHeaders });
        }



        #endregion
    }
}
