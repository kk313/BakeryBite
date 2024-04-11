using BakeryBite.Models;

using NUnit.Framework;

using System.Linq;

namespace BakeryBite.Data.Tests
{
    [TestFixture]
    public class ShoppingCartTests
    {
        private Product product1;
        private Product product2;
        private ShoppingCart cart;

        [SetUp]
        public void Setup()
        {
            // Инициализация тестовых данных
            product1 = new Product { Id = 1, Name = "Товар 1", Cost = 10.99m };
            product2 = new Product { Id = 2, Name = "Товар 2", Cost = 5.99m };
            cart = new ShoppingCart();
        }

        //[Test]
        //public void AddItem_NewProduct_ShouldIncreaseItemCount()
        //{
        //    // Действие
        //    cart.AddItem(product1, 2);

        //    // Утверждение
        //    var items = cart.GetItems();
        //    Assert.Equals(1, items.Count);
        //    Assert.Equals(2, items.First().Quantity);
        //}

        //[Test]
        //public void AddItem_ExistingProduct_ShouldIncreaseQuantity()
        //{
        //    // Добавляем товар в корзину
        //    cart.AddItem(product1, 2);

        //    // Добавляем еще товар того же типа
        //    cart.AddItem(product1, 3);

        //    // Проверяем, что количество увеличилось
        //    var items = cart.GetItems();
        //    Assert.Equals(1, items.Count);
        //    Assert.Equals(5, items.First().Quantity); // 2 + 3 = 5
        //}

        [Test]
        public void RemoveItem_ExistingProduct_ShouldRemoveFromCart()
        {
            // Добавляем товар в корзину
            cart.AddItem(product1, 2);

            // Удаляем товар из корзины
            cart.RemoveItem(product1.Id);

            // Проверяем, что корзина пуста
            var items = cart.GetItems();
            Assert.That(items, Is.Empty);
        }

        [Test]
        public void GetTotal_ShouldCalculateCorrectTotal()
        {
            // Добавляем два разных товара в корзину
            cart.AddItem(product1, 2);
            cart.AddItem(product2, 1);

            // Проверяем, что общая стоимость правильно рассчитана
            var total = cart.GetTotal();
            Assert.Equals(2 * 10.99m + 1 * 5.99m, total);
        }
    }
}
