namespace reiderproject1.Models;
public class User
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string PasswordSalt { get; set; } = "";

    public List<CartItem> Cart { get; set; } = new();

    public User() { }

    public User(string fullName, string email, string passwordHash,
        string passwordSalt)
    {
        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }
}