using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net;
using System.IO;
using System.Web.Mvc;
using UPAEventsPayPal;
using Twitter;
using MartinLayooInc.Services.Interfaces;
using MartinLayooIncs.UnitOfWork;
using MartinLayooInc.DomainModels.Domain;
using System.Text;
using System.Net.Mail;
using MartinLayooInc.Caching;
using MartinLayooInc.Infrastructure;
using MartinLayooInc.Services.Concretes;
using System.Text.RegularExpressions;
using MartinLayooInc.SocialMedia;
using log4net;
using MartinLayooInc.Models;

namespace MartinLayooInc.Controllers
{
    public class HomeController : Controller
    {
        UnitOfWork _unitOfWork;
        private AdhocRepository _adhocRepository;
        private ProductRepository _productRepository;

        private ILog _logger;
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            _logger = log4net.LogManager.GetLogger(this.GetType());
            //Log the error!!
            _logger.Error(filterContext.Exception);
            _logger.Error(filterContext.Exception.StackTrace);

            //Redirect or return a view, but not both.
            filterContext.Result = RedirectToAction("Error", "Error");
        }
        public HomeController()
        {
            _logger = log4net.LogManager.GetLogger(this.GetType());
        }
        public HomeController(AddressRepository addressRepository, ProductRepository productRepository, BookingRepository bookingRepository, BookedItemRepository bookedItemRepository, LocationRepository locationRepository, PaymentRepository paymentRepository, UserRepository userRepository, PostItemRepository postItemRepository, AdhocRepository adhocRepository)
        {
            var dbContext = new DataAccess.Concretes.MartinLayooIncDBContext();
            adhocRepository.Context = dbContext;
            addressRepository.Context = dbContext;
            productRepository.Context = dbContext;
            bookingRepository.Context = dbContext;
            bookedItemRepository.Context = dbContext;
            locationRepository.Context = dbContext;
            paymentRepository.Context = dbContext;
            userRepository.Context = dbContext;
            postItemRepository.Context = dbContext;
            _adhocRepository = adhocRepository;
            _productRepository = productRepository;
            _unitOfWork = new UnitOfWork(addressRepository, productRepository, bookedItemRepository, locationRepository, paymentRepository, userRepository, postItemRepository, bookingRepository);
            _logger = log4net.LogManager.GetLogger(this.GetType());
        }

        public ActionResult Index()
        {
            GetPartialViewData();
            ViewBag.Title = "MartinLayooInc. Security Software";
            var model = GetContentByPageTitle(ViewBag.Title);

            return View(model ?? new PostItem { Title = "MartinLayooInc. Security Software", Description = "MartinLayooInc. Security Software" });
        }

        [HttpGet]
        public ActionResult ViewPagedProducts()
        {
            return View("ProductPaged");
        }
        [HttpPost]
        public JsonResult GetPagedProducts(PagedProductsParams queryParams)
        {
            var elements = queryParams.numberPerPage * queryParams.pagesToShow;

            var startPage = 0;

            startPage = queryParams.viewStartPage * queryParams.numberPerPage;

            var skip = startPage - queryParams.numberPerPage;
            
            var items = _unitOfWork.BookingRepository.Context.Shop_Prods.ToArray().Reverse().Skip(skip).Take(elements);
            var result = new List<SHOP_PRODS>();
            foreach (var item in items)
            {
                item.prodDesc = HttpContext.Server.HtmlEncode(item.prodDesc);
                result.Add(item);
            }
            return Json(result.ToArray());
        }
    }
}