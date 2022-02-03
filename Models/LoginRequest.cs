namespace TestApi.Models;

public class LoginRequest
{
    public long Id { get; set; }

    public string Password { get; set; } = null!;
}