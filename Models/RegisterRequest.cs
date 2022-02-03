namespace TestApi.Models;

public class RegisterRequest
{
    public string Title { get; set; } = null!;

    public string Password { get; set; } = null!;
}