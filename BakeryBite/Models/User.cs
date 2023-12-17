namespace BakeryBite.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Status {  get; set; }

        public User (int id, string name, string password, string status)
        {
            Id = id;
            Name = name;
            Password = password;
            Status = status;
        }
    }
}
