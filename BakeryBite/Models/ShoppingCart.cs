namespace BakeryBite.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public int CartId { get; set; }
        public Item Item { get; set; }
        public User User { get; set; }

        public ShoppingCart(int id, int itemId, int userId, int cartId, Item item, User user)
        {
            Id = id;
            ItemId = itemId;
            UserId = userId;
            CartId = cartId;
            Item = item;
            User = user;
        }
    }
}
