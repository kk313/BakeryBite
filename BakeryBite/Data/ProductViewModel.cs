using BakeryBite.Models;

using System.ComponentModel.DataAnnotations;

namespace BakeryBite.Data
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public List<Category> Categories { get; set; }
        public IFormFile Image { get; set; }
    }
}
