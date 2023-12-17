namespace BakeryBite.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public Item (int id, string name, decimal weight, decimal cost, string description, string avatar, int categoryId, Category category)
        {
            Id = id;
            Name = name;
            Weight = weight;
            Cost = cost;
            Description = description;
            Avatar = avatar;
            CategoryId = categoryId;
            Category = category;
        }
    }
}
