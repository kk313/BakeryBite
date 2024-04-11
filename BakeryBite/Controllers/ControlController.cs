using BakeryBite.Data;
using BakeryBite.Models;

using Microsoft.AspNetCore.Mvc;

using System.Text.RegularExpressions;

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

        public IActionResult OrderEditor() => View();

        public IActionResult ProductsEditor()
        {
            var products = _context.Product.ToList();
            return View(products);
        }

        public IActionResult ProductOneEditor(int productId)
        {
            var product = _context.Product.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                Product = product,
                Categories = _context.Category.ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ProductOneAdder()
        {
            var categories = _context.Category.ToList();
            var viewModel = new ProductViewModel
            {
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddProduct(ProductViewModel viewModel)
        {
            if (!CheckInput(viewModel))
            {
                viewModel.Categories = _context.Category.ToList();
                return View("ProductOneAdder", viewModel);
            }

            try
            {
                var product = new Product
                {
                    Name = viewModel.Product.Name,
                    Weight = viewModel.Product.Weight,
                    Description = viewModel.Product.Description,
                    Cost = viewModel.Product.Cost,
                    CategoryId = viewModel.Product.CategoryId
                };

                if (viewModel.Image != null && viewModel.Image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        viewModel.Image.CopyTo(memoryStream);
                        product.Avatar = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }

                _context.Product.Add(product);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Новый товар успешно добавлен.";

                return RedirectToAction("ProductsEditor");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ошибка сохранения данных: {ex.Message}");
                viewModel.Categories = _context.Category.ToList();
                return View("ProductOneAdder", viewModel);
            }
        }

        [HttpPost]
        public IActionResult Edit(ProductViewModel viewModel)
        {
            if (!CheckInput(viewModel))
            {
                viewModel.Categories = _context.Category.ToList();
                return View("ProductOneEditor", viewModel);
            }

            var product = _context.Product.FirstOrDefault(p => p.Id == viewModel.Product.Id);

            if (product == null)
            {
                return NotFound();
            }

            product.Name = viewModel.Product.Name;
            product.Weight = viewModel.Product.Weight;
            product.Description = viewModel.Product.Description;
            product.Cost = viewModel.Product.Cost;
            product.CategoryId = viewModel.Product.CategoryId;

            if (viewModel.Image != null && viewModel.Image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    viewModel.Image.CopyTo(memoryStream);
                    product.Avatar = Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            else
            {
                viewModel.Product.Avatar = product.Avatar;
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Товар успешно отредактирован.";

            return RedirectToAction("ProductsEditor");
        }

        private bool CheckInput(ProductViewModel viewModel)
        {
            // Проверка на пустоту полей и допустимость символов
            if (string.IsNullOrWhiteSpace(viewModel.Product.Name) ||
                string.IsNullOrWhiteSpace(viewModel.Product.Description) ||
                string.IsNullOrWhiteSpace(viewModel.Product.Weight.ToString()) ||
                string.IsNullOrWhiteSpace(viewModel.Product.Cost.ToString()) ||
                !Regex.IsMatch(viewModel.Product.Name, "^[а-яА-Яa-zA-Z ]+$") ||
                !Regex.IsMatch(viewModel.Product.Description, "^[а-яА-Яa-zA-Z ]+$") ||
                !Regex.IsMatch(viewModel.Product.Weight.ToString(), @"^[0-9]+$") ||
                !Regex.IsMatch(viewModel.Product.Cost.ToString(), @"^[0-9]+$"))
            {
                ModelState.AddModelError(string.Empty, "Пожалуйста, заполните все обязательные поля корректно.");
                return false;
            }

            // Проверка длины названия и описания
            if (viewModel.Product.Name.Length > 32)
            {
                ModelState.AddModelError(nameof(viewModel.Product.Name), "Название товара не должно превышать 32 символа.");
                return false;
            }

            if (viewModel.Product.Description.Length > 64)
            {
                ModelState.AddModelError(nameof(viewModel.Product.Description), "Описание товара не должно превышать 64 символа.");
                return false;
            }

            return true;
        }
    }
}
