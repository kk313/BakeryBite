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
    }
}
