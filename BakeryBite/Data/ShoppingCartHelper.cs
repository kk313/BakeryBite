using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

namespace BakeryBite.Data
{
    public class ShoppingCartHelper
    {
        private const string CartSessionKey = "Cart";

        public static ShoppingCart GetCart(HttpContext context)
        {
            var cartJson = context.Session.GetString(CartSessionKey);

            if (cartJson != null)
            {
                return JsonConvert.DeserializeObject<ShoppingCart>(cartJson);
            }

            return new ShoppingCart();
        }

        public static void SaveCart(HttpContext context, ShoppingCart cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            context.Session.SetString(CartSessionKey, cartJson);
        }
    }
}
