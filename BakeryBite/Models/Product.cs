using System.ComponentModel.DataAnnotations;

namespace BakeryBite.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public string? Avatar { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int StockQuantity { get; set; }
        public bool IsHidden { get; set; }
    }
}
