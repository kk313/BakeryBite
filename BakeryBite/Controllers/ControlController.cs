using BakeryBite.Data;
using BakeryBite.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult OrderEditor()
        {
            List<Order> orders = _context.Order.Where(o => o.IsCompleted == 0).ToList();
            return View(orders);
        }

        public IActionResult OrderConfirmed()
        {
            List<Order> confirmedOrders = _context.Order.Where(o => o.IsCompleted == 1).ToList();
            return View(confirmedOrders);
        }

        public IActionResult OrderRejected()
        {
            List<Order> rejectedOrders = _context.Order.Where(o => o.IsCompleted == 2).ToList();
            return View(rejectedOrders);
        }

        public IActionResult OrderItems(int orderId)
        {
            var orderItems = _context.OrderItem
                                     .Include(oi => oi.Product)
                                     .Where(oi => oi.OrderId == orderId)
                                     .ToList();

            return View(orderItems);
        }

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

        [HttpPost]
        public IActionResult DeleteProduct(int productId)
        {
            var product = _context.Product.FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            try
            {
                var isProductInOrders = _context.OrderItem.Any(oi => oi.ProductId == productId);

                if (isProductInOrders)
                {
                    return Json(new { success = false, errorMessage = "Невозможно удалить товар, так как он уже используется в заказах." });
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Product.Remove(product);
                        _context.SaveChanges();
                        transaction.Commit();

                        return Json(new { success = true });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, errorMessage = "Ошибка при удалении товара: " + ex.Message });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Ошибка при проверке наличия товара в заказах: " + ex.Message });
            }
        }


        [HttpPost("updateorderstatus")]
        public IActionResult UpdateOrderStatus(int orderId, int status)
        {
            var order = _context.Order.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.IsCompleted = status;
                _context.SaveChanges();
            }

            return RedirectToAction("OrderEditor");
        }

        public string GetOrderStatus(int status)
        {
            switch (status)
            {
                case 1:
                    return "Оформлен";
                case 2:
                    return "Отклонён";
                default:
                    return "Не завершён";
            }
        }

        private bool CheckInput(ProductViewModel viewModel)
        {
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
