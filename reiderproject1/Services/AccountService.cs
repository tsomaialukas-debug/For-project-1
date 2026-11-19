namespace reiderproject1.Services;
using reiderproject1.Models;

public class AccountService
{
    public List<User> Users = new();
    public User? CurrentUser { get; private set; }

    public void Register()
    {
        Console.Write("Full name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";

        if (Users.Any(u => u.Email == email))
        {
            Console.WriteLine("This email is already registered!");
            return;
        }

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("All fields are required!");
            return;
        }

        var newUser = new User(name, email, password);

        Users.Add(newUser);
        CurrentUser = newUser;

        Console.WriteLine("Registration successful!");
        Console.WriteLine($"Welcome, {CurrentUser.FullName}!");
    }

    public void Login()
    {
        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? "";

        CurrentUser = Users.FirstOrDefault(
            u => u.Email == email && u.Password == password);

        Console.WriteLine(CurrentUser != null
            ? $"Welcome, {CurrentUser.FullName}!"
            : "Incorrect email or password!");
    }

    public void Logout()
    {
        CurrentUser = null;
        Console.WriteLine("You have logged out!");
    }
}