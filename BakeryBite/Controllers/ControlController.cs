using BakeryBite.Data;
using BakeryBite.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

using System.Globalization;
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


        // Пользовательская часть
        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Authorize", "Home");
            }

            var user = _context.User.Include(u => u.Role).FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return RedirectToAction("Authorize", "Home");
            }

            return View(user);
        }

        public IActionResult Logout()
        {
            var emptyModel = new LoginViewModel();

            HttpContext.Session.Remove("UserId");

            return RedirectToAction("Authorize", "Home", emptyModel);
        }

        public IActionResult ProfileEditor()
        {
            if (!HttpContext.Session.TryGetValue("UserId", out byte[] userIdBytes))
            {
                return NotFound();
            }

            int userId = (int)HttpContext.Session.GetInt32("UserId");

            var user = _context.User.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserProfileViewModel
            {
                User = user
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult UpdateUser(UserProfileViewModel model)
        {
            var user = _context.User.FirstOrDefault(u => u.Email == model.User.Email);

            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(model.User.Name) && user.Name != model.User.Name)
            {
                user.Name = model.User.Name;
            }

            if (!string.IsNullOrWhiteSpace(model.User.Surname) && user.Surname != model.User.Surname)
            {
                user.Surname = model.User.Surname;
            }

            if (!string.IsNullOrWhiteSpace(model.User.Patronymic) && user.Patronymic != model.User.Patronymic)
            {
                user.Patronymic = model.User.Patronymic;
            }
            else { user.Patronymic = null; }

            if (!string.IsNullOrWhiteSpace(model.User.Email) && user.Email != model.User.Email)
            {
                var existingUserWithEmail = _context.User.FirstOrDefault(u => u.Email == model.User.Email);
                if (existingUserWithEmail != null && existingUserWithEmail.Id != user.Id)
                {
                    ModelState.AddModelError("User.Email", "Пользователь с таким Email уже существует.");
                    return View("ProfileEditor", model);
                }
                user.Email = model.User.Email;
            }

            if (!string.IsNullOrWhiteSpace(model.User.Phone) && user.Phone != model.User.Phone)
            {
                var existingUserWithPhone = _context.User.FirstOrDefault(u => u.Phone == model.User.Phone);
                if (existingUserWithPhone != null && existingUserWithPhone.Id != user.Id)
                {
                    ModelState.AddModelError("User.Phone", "Пользователь с таким номером телефона уже существует.");
                    return View("ProfileEditor", model);
                }
                user.Phone = model.User.Phone;
            }

            if (!string.IsNullOrWhiteSpace(model.User.Password))
            {
                if (string.IsNullOrWhiteSpace(model.OldPassword) || model.OldPassword != user.Password)
                {
                    ModelState.AddModelError("OldPassword", "Текущий пароль неверен.");
                    return View("ProfileEditor", model);
                }

                user.Password = model.User.Password;
            }

            _context.SaveChanges();

            return RedirectToAction("Profile", "Control");
        }

        // Заказы
        public IActionResult OrderEditor()
        {
            List<Order> orders = _context.Order
                .Include(o => o.User)  
                .Where(o => o.IsCompleted == 0)
                .ToList();

            return View(orders);
        }

        public IActionResult OrderConfirmed()
        {
            List<Order> confirmedOrders = _context.Order
                .Include(o => o.User)  
                .Where(o => o.IsCompleted == 1)
                .ToList();

            return View(confirmedOrders);
        }

        public IActionResult OrderRejected()
        {
            List<Order> rejectedOrders = _context.Order
                .Include(o => o.User)  
                .Where(o => o.IsCompleted == 2)
                .ToList();

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

        public IActionResult UserOrders(int userId)
        {
            List<Order> userOrders = _context.Order.Where(o => o.UserId == userId).ToList();
            return View("UserOrders", userOrders);
        }

        public IActionResult UpdateOrderStatus(int orderId, int status)
        {
            var order = _context.Order
                                .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.Product)
                                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Заказ не найден." });
            }

            if (status == 1)
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.Product.StockQuantity < item.Quantity)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"Недостаточно товара '{item.Product.Name}' на складе. Доступно: {item.Product.StockQuantity}, необходимо: {item.Quantity}."
                        });
                    }
                }

                foreach (var item in order.OrderItems)
                {
                    item.Product.StockQuantity -= item.Quantity;
                    if (item.Product.StockQuantity == 0)
                    {
                        item.Product.IsHidden = true;
                    }
                }
            }

            order.IsCompleted = status;
            _context.SaveChanges();

            string toastrMessage = status == 1 ? "Заказ подтвержден" : "Заказ отклонен";
            return Json(new { success = true, message = toastrMessage });
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

        // Товары
        public IActionResult ProductsEditor(string searchTerm = "", int page = 1, int pageSize = 9)
        {
            var products = _context.Product.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                products = products.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            var totalItems = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var items = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new FoodViewModel
            {
                Products = items,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                ActionName = nameof(ProductsEditor),
                SearchTerm = searchTerm
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ToggleVisibility(int productId)
        {
            var product = _context.Product.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            if (product.StockQuantity == 0)
            {
                return BadRequest("Нельзя изменить видимость товара с нулевым количеством на складе.");
            }

            product.IsHidden = !product.IsHidden;
            _context.SaveChanges();

            return Ok();
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
            if (string.IsNullOrWhiteSpace(viewModel.Product.Name))
            {
                ModelState.AddModelError("Product.Name", "Название товара не может содержать только пробелы.");
            }

            if (string.IsNullOrWhiteSpace(viewModel.Product.Description))
            {
                ModelState.AddModelError("Product.Description", "Описание товара не может содержать только пробелы.");
            }

            if (viewModel.Product.CategoryId == 0)
            {
                ModelState.AddModelError("Product.CategoryId", "Категория товара не может быть пустой.");
            }

            if (viewModel.Product.Weight < 150)
            {
                ModelState.AddModelError("Product.Weight", "Вес товара не может быть меньше 150 грамм.");
            }

            if (viewModel.Product.Cost < 50)
            {
                ModelState.AddModelError("Product.Cost", "Стоимость товара не может быть меньше 50 рублей.");
            }

            if (viewModel.Product.CategoryId == 0 || string.IsNullOrWhiteSpace(viewModel.Product.Description) 
                || string.IsNullOrWhiteSpace(viewModel.Product.Name) || viewModel.Product.Weight < 150 
                || viewModel.Product.Cost < 50)
            {
                viewModel.Categories = _context.Category.ToList();
                return View("ProductOneAdder", viewModel); 
            }

            try
            {
                var product = new Product
                {
                    Name = viewModel.Product.Name.Trim(),
                    Weight = viewModel.Product.Weight,
                    Description = viewModel.Product.Description.Trim(),
                    Cost = viewModel.Product.Cost,
                    CategoryId = viewModel.Product.CategoryId,
                    StockQuantity = 0,
                    IsHidden = true
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
            var product = _context.Product.FirstOrDefault(p => p.Id == viewModel.Product.Id);

            if (product == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(viewModel.Product.Name))
            {
                ModelState.AddModelError("Product.Name", "Название товара не может содержать только пробелы.");
            }

            if (string.IsNullOrWhiteSpace(viewModel.Product.Description))
            {
                ModelState.AddModelError("Product.Description", "Описание товара не может содержать только пробелы.");
            }

            if (viewModel.Product.Weight < 150)
            {
                ModelState.AddModelError("Product.Weight", "Вес товара не может быть меньше 150 грамм.");
            }

            if (viewModel.Product.Cost < 50)
            {
                ModelState.AddModelError("Product.Cost", "Стоимость товара не может быть меньше 50 рублей.");
            }

            if (string.IsNullOrWhiteSpace(viewModel.Product.Description) || string.IsNullOrWhiteSpace(viewModel.Product.Name) 
                || viewModel.Product.Weight < 150 || viewModel.Product.Cost < 50)
            {
                viewModel.Categories = _context.Category.ToList();
                return View("ProductOneEditor", viewModel);
            }

            product.Name = viewModel.Product.Name;
            product.Weight = viewModel.Product.Weight;
            product.Description = viewModel.Product.Description;
            product.Cost = viewModel.Product.Cost;
            product.CategoryId = viewModel.Product.CategoryId;
            product.StockQuantity = viewModel.Product.StockQuantity;

            if (viewModel.Product.StockQuantity == 0)
            {
                product.IsHidden = true;
            }

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
        public IActionResult DeleteProduct([FromBody] int productId)
        {
            var product = _context.Product.FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return Json(new { success = false, errorMessage = "Товар не найден." });
            }

            var isUsedInOrders = _context.OrderItem.Any(oi => oi.ProductId == productId);
            if (isUsedInOrders)
            {
                return Json(new { success = false, errorMessage = "Невозможно удалить товар, так как он используется в заказах." });
            }

            try
            {
                _context.Product.Remove(product);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Ошибка при удалении товара: " + ex.Message });
            }
        }

        
    }
}
