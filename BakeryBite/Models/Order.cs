namespace BakeryBite.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsCompleted { get; set; }
        public int Phone { get; set; }
        public string Address { get; set; }
    }
}
