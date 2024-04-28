﻿using BakeryBite.Models;
using BakeryBite.Data;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BakeryBite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, ApplicationContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            var categoryViewModels = new List<CategoryViewModel>();

            var categoryIds = _context.Product.Select(p => p.CategoryId).Distinct().ToList();

            foreach (var categoryId in categoryIds)
            {
                string categoryName = $"Food{categoryId}";

                var randomProduct = _context.Product
                    .Where(p => p.CategoryId == categoryId && !string.IsNullOrEmpty(p.Avatar))
                    .OrderBy(p => Guid.NewGuid())
                    .FirstOrDefault();

                if (randomProduct != null)
                {
                    var categoryViewModel = new CategoryViewModel
                    {
                        CategoryName = categoryName,
                        CategoryRuName = GetCategoryRuName(categoryName),
                        Avatar = randomProduct.Avatar
                    };

                    categoryViewModels.Add(categoryViewModel);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"No product found for category {categoryName}");
                }
            }

            return View(categoryViewModels);
        }

        private string GetCategoryRuName(string categoryName)
        {
            if (categoryName.StartsWith("Food"))
            {
                if (int.TryParse(categoryName.Substring(4), out int categoryId))
                {
                    var category = _context.Category.FirstOrDefault(c => c.Id == categoryId);
                    if (category != null)
                    {
                        return category.Name;
                    }
                }
            }
            return categoryName;
        }

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

        public IActionResult Registration()
        {
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

            HttpContext.Session.SetInt32("UserId", user.Id);

            return RedirectToAction("Profile", "Control");
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            var existingEmail = _context.User.FirstOrDefault(u => u.Email == user.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Пользователь с таким Email уже существует.");
                return View("Registration", user);
            }

            var existingPhone = _context.User.FirstOrDefault(u => u.Phone == user.Phone);
            if (existingPhone != null)
            {
                ModelState.AddModelError("Phone", "Пользователь с таким номером телефона уже существует.");
                return View("Registration", user);
            }

            var existingLogin = _context.User.FirstOrDefault(u => u.Login == user.Login);
            if (existingLogin != null)
            {
                ModelState.AddModelError("Login", "Пользователь с таким логином уже существует.");
                return View("Registration", user);
            }

            try
            {
                user.RoleId = 4;
                _context.User.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Authorize");
            }

            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ошибка сохранения данных: {ex.Message}");
            }
            return View("Registration", user);
        }


        public IActionResult ShoppingCart()
        {
            ShoppingCart cart = ShoppingCartHelper.GetCart(HttpContext);

            List<CartItem> cartItems = cart.items;

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            Product product = _context.Product.FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            ShoppingCart cart = ShoppingCartHelper.GetCart(HttpContext);

            CartItem existingItem = cart.items.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.items.Add(new CartItem
                {
                    ProductId = productId,
                    Product = product,
                    Quantity = quantity
                });
            }

            ShoppingCartHelper.SaveCart(HttpContext, cart);

            return Ok();
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            ShoppingCart cart = ShoppingCartHelper.GetCart(HttpContext);

            CartItem itemToRemove = cart.items.FirstOrDefault(item => item.ProductId == productId);

            if (itemToRemove != null)
            {
                if (itemToRemove.Quantity > 1)
                {
                    itemToRemove.Quantity -= 1;
                }
                else
                {
                    cart.items.Remove(itemToRemove);
                }

                ShoppingCartHelper.SaveCart(HttpContext, cart);

                return Ok();
            }

            return NotFound();
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmOrder(OrderConfirmationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                if (userId != null)
                {
                    var user = _context.User.FirstOrDefault(u => u.Id == userId);

                    if (user != null)
                    {
                        model.Name = user.Name;
                        model.Email = user.Email;
                        model.Phone = user.Phone.ToString(); 
                    }
                }

                var newOrder = new Order
                {
                    OrderDate = DateTime.Now,
                    TotalAmount = CalculateTotalAmount(), 
                    IsCompleted = 0,
                    Phone = Convert.ToInt64(model.Phone),
                    Address = model.Address,
                    Name = model.Name,
                    Email = model.Email,
                    PaymentMethod = model.PaymentMethod.ToString(),
                    UserId = HttpContext.Session.GetInt32("UserId")
                };

                _context.Order.Add(newOrder);
                _context.SaveChanges();

                var cart = ShoppingCartHelper.GetCart(HttpContext);
                foreach (var cartItem in cart.items)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = newOrder.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity
                    };
                    _context.OrderItem.Add(orderItem);
                }

                _context.SaveChanges();

                cart.items.Clear();
                ShoppingCartHelper.SaveCart(HttpContext, cart);

                return RedirectToAction("Index", "Home");
            }

            return View("OrderConfirmation", model);
        }


        private decimal CalculateTotalAmount()
        {
            var cart = ShoppingCartHelper.GetCart(HttpContext);
            return cart.items.Sum(item => item.Quantity * item.Product.Cost);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}