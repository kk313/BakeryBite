namespace BakeryBite.Data
{
    public class OrderConfirmationViewModel
    {
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }

    public enum PaymentMethod
    {
        Картой,
        Наличными
    }
}
