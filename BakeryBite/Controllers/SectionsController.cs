using BakeryBite.Models;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace BakeryBite.Controllers
{
    public class SectionsController : Controller
    {
        private readonly ILogger<SectionsController> _logger;

        public SectionsController(ILogger<SectionsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Contacts() => View();

        public IActionResult AboutUs() => View();

        public IActionResult FAQ() => View();

        public IActionResult PrivacyPolicy() => View();

        public IActionResult PublicOffer() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}