using BakeryBite.Models;

namespace BakeryBite.Data
{
    public class FoodViewModel
    {
        public Category Category { get; set; }
        public List<Product> Products { get; set; }
    }
}
