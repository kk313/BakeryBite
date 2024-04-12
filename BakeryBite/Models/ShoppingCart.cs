using BakeryBite.Models;

using NUnit.Framework.Interfaces;

using System.ComponentModel.DataAnnotations;

public class ShoppingCart
{
    public int Id { get; set; }
    public List<CartItem> items;

    public ShoppingCart()
    {
        items = new List<CartItem>();
    }
}