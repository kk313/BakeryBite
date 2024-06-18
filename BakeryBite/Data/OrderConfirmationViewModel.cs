using System.ComponentModel.DataAnnotations;

namespace BakeryBite.Data
{
    public class OrderConfirmationViewModel
    {
        [Required(ErrorMessage = "Адрес доставки не может содержать только пробелы.")]
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PaymentMethod { get; set; }
        public int? RoleId { get; set; }
    }
}
