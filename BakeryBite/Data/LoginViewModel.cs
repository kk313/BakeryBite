using System.ComponentModel.DataAnnotations;

namespace BakeryBite.Data
{
    public class LoginViewModel
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? UserRole { get; set; }
    }
}
