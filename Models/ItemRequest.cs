using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TestApi.Models;

public class ItemRequest
{
    public string Name { get; set; } = null!;

    public int Value { get; set; }
}