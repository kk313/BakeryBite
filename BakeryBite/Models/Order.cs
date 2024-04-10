namespace BakeryBite.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int CartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsRejected { get; set; }
    }
}
