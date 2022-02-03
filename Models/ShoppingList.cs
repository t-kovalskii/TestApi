using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TestApi.Models;

public class ShoppingList
{
    // Primary key
    [BindingBehavior(BindingBehavior.Optional)]
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string Password { get; set; } = null!;

    public List<Item> Items { get; set; } = new List<Item>();
}