using BakeryBite.Models;
using BakeryBite.Data;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace BakeryBite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;
        private readonly ShoppingCart _shoppingCart;

        public HomeController(ILogger<HomeController> logger, ApplicationContext context, ShoppingCart shoppingCart)
        {
            _logger = logger;
            _context = context;
            _shoppingCart = shoppingCart;
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var productToAdd = _context.Product.FirstOrDefault(p => p.Id == productId);

            if (productToAdd != null)
            {
                _shoppingCart.AddItem(productToAdd, 1);
                Console.WriteLine($"Товар добавлен в корзину: {productToAdd.Name}");
                HttpContext.Session.SetObject("ShoppingCart", _shoppingCart);
            }
            return RedirectToAction("ShoppingCart");
        }

        public IActionResult ShoppingCart()
        {
            var shoppingCart = HttpContext.Session.GetObject<ShoppingCart>("ShoppingCart");

            if (shoppingCart == null || shoppingCart.GetItems().Count() == 0)
            {
                return View(new List<CartItem>());
            }

            var itemsInCart = shoppingCart.GetItems().ToList();
            return View(itemsInCart);
        }

        public IActionResult Index() => View();

        public IActionResult Food1()
        {
            var foodItems = _context.Product
            .Where(p => p.CategoryId == 1)
            .ToList();
            return View(foodItems);
        }

        public IActionResult Food2()
        {
            var foodItems = _context.Product
            .Where(p => p.CategoryId == 2)
            .ToList();
            return View(foodItems);
        }

        public IActionResult Food3()
        {
            var foodItems = _context.Product
            .Where(p => p.CategoryId == 3)
            .ToList();
            return View(foodItems);
        }

        public IActionResult Food4()
        {
            var foodItems = _context.Product
            .Where(p => p.CategoryId == 4)
            .ToList();
            return View(foodItems);
        }

        public IActionResult Food5()
        {
            var foodItems = _context.Product
            .Where(p => p.CategoryId == 5)
            .ToList();
            return View(foodItems);
        }

        public IActionResult Food6()
        {
            var foodItems = _context.Product
            .Where(p => p.CategoryId == 6)
            .ToList();
            return View(foodItems);
        }

        public IActionResult Authorize()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                return RedirectToAction("Profile", "Control");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string userName, string password)
        {
            var user = _context.User.FirstOrDefault(u => u.Login == userName && u.Password == password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Неверные учетные данные");
                return View("Authorize");
            }

            HttpContext.Session.SetString("UserName", user.Name.ToString());
            HttpContext.Session.SetString("UserRole", user.RoleId.ToString());

            return RedirectToAction("Profile", "Control");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}