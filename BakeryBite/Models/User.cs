using System.ComponentModel.DataAnnotations;

namespace BakeryBite.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Status {  get; set; }
    }
}
