using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace TrackMe.Areas.Customer.Controllers
{
    [Area("Customer")]          //Attribute , Here we are Displaying all Products so we need 
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productList);
        }

        //public IActionResult Details(int productid)
        //{

        //    Product product = _unitOfWork.Product.Get(u => u.Id == productid, includeProperties: "Category");
        //    return View(product);
        //}
        public IActionResult Details(int productid)
        {
            ShoppingCart cart = new ShoppingCart
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productid, includeProperties: "Category"),
                Count = 1,
                ProductId = productid
            };

            return View(cart);
        }

        [HttpPost]
        [Authorize]                     // we need user ID to store and they must be autorized user
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u=> u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);

            if(cartFromDb != null)
            {
                //Shopping cart already exist
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.update(cartFromDb);
                TempData["success"] = "Cart Updated SuccessFully";
            }
            else
            {
                //Cart to be added
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                TempData["success"] = "Cart Added SuccessFully";
            }

           
            _unitOfWork.Save();
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
