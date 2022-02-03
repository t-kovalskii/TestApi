using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace TestApi.Models;

public class Item 
{
    // Primary key
    [BindingBehavior(BindingBehavior.Optional)]
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public int Value { get; set; }

    [BindingBehavior(BindingBehavior.Optional)]
    public long ShoppingListId { get; set; }

    [JsonIgnore]
    public ShoppingList ShoppingList { get; set; } = null!;
}