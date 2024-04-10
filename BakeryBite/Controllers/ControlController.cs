using BakeryBite.Data;

using Microsoft.AspNetCore.Mvc;

namespace BakeryBite.Controllers
{
    public class ControlController : Controller
    {
        private readonly ILogger<ControlController> _logger;
        private readonly ApplicationContext _context;

        public ControlController(ILogger<ControlController> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Profile()
        {
            string? userName = HttpContext.Session.GetString("UserName");
            string? userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userRole))
            {
                return RedirectToAction("Authorize", "Home");
            }

            var role = _context.Role.FirstOrDefault(r => r.Id == int.Parse(userRole));

            if (role == null)
            {
                return RedirectToAction("Authorize", "Home");
            }

            var profileModel = new LoginViewModel
            {
                UserName = userName,
                UserRole = role.Name
            };

            return View(profileModel);
        }

        public IActionResult Logout()
        {
            var emptyModel = new LoginViewModel();

            HttpContext.Session.Clear();

            return RedirectToAction("Authorize", "Home", emptyModel);
        }
    }
}
