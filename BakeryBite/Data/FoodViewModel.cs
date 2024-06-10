using BakeryBite.Models;

namespace BakeryBite.Data
{
    public class FoodViewModel
    {
        public Category Category { get; set; }
        public List<Product> Products { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string ActionName { get; set; }
        public string SearchTerm { get; set; }
    }
}
