using BakeryBite.Models;

namespace BakeryBite.Data
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public string? OldPassword { get; set; }
    }
}
