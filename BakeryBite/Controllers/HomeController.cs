using BakeryBite.Models;
using BakeryBite.Data;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace BakeryBite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
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
            if (HttpContext.Session.GetString("UserId") != null)
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

            HttpContext.Session.SetString("UserId", user.Id.ToString());
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