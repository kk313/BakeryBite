﻿using System.ComponentModel.DataAnnotations;

namespace BakeryBite.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int IsCompleted { get; set; }
        public long Phone { get; set; }
        public string Address { get; set; }
        public string Name { get; set; } 
        public string Email { get; set; } 
        public string PaymentMethod { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
