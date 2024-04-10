using BakeryBite.Models;
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

        //public IActionResult Food1()
        //{
        //    var foodItems = _context.Item
        //.Where(item => item.CategoryId == 1)
        //.ToList();

        //    return View(foodItems);
        //}

        public IActionResult Index() => View();
        public IActionResult Food1() => View();
        public IActionResult Food2() => View();
        public IActionResult Food3() => View();
        public IActionResult Food4() => View();
        public IActionResult Food5() => View();
        public IActionResult Food6() => View();
        public IActionResult Authorize() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}