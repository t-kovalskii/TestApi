using System.Text;

namespace TestApi;

public static class StringHelpers
{
    public static string ToBase64(string s)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        return Convert.ToBase64String(bytes);
    }

    public static string FromBase64(string s)
    {
        var bytes = Convert.FromBase64String(s);
        return Encoding.UTF8.GetString(bytes);
    }
}