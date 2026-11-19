namespace reiderproject1.Models;

public class User
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public List<CartItem> Cart { get; set; } = new();

    public User(string fullName, string email, string password)
    {
        FullName = fullName;
        Email = email;
        Password = password;
    }
}