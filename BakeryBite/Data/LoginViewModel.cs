using System.ComponentModel.DataAnnotations;

namespace BakeryBite.Data
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Логин обязателен для входа")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Пароль обязателен для входа")]
        public string? Password { get; set; }

        public string? UserRole { get; set; }
    }
}
