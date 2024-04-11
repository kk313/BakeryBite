using BakeryBite.Models;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using NUnit.Framework.Interfaces;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public class ShoppingCart
{
    [Key]
    public int Id { get; set; }

    private List<CartItem> items;

    public ShoppingCart()
    {
        items = new List<CartItem>();
    }

    public void AddItem(Product product, int quantity)
    {
        var existingItem = items.FirstOrDefault(item => item.Product.Id == product.Id);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            items.Add(new CartItem { Product = product, Quantity = quantity });
        }
    }

    public void RemoveItem(int productId)
    {
        var itemToRemove = items.FirstOrDefault(item => item.ProductId == productId);
        if (itemToRemove != null)
        {
            items.Remove(itemToRemove);
        }
    }

    public IEnumerable<CartItem> GetItems()
    {
        return items;
    }

    public void ClearCart()
    {
        items.Clear();
    }

    public decimal GetTotal()
    {
        return items.Sum(item => item.Product.Cost * item.Quantity);
    }
}
