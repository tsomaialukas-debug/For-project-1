namespace reiderproject1.Services;
using reiderproject1.Models;
using System.Text.Json;
using System.Security.Cryptography;

public class AccountService
{
    public List<User> Users = new();
    public User? CurrentUser { get; private set; }

    private readonly string folder =
        Path.Combine(Directory.GetCurrentDirectory(), "Data");

    private string FilePath =>
        Path.Combine(folder, "users.json");

    public AccountService()
    {
        LoadUsers();
    }

    // Save accounts
    private void SaveUsers()
    {
        Directory.CreateDirectory(folder);

        string json = JsonSerializer.Serialize(
            Users,
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(FilePath, json);
    }

    // Load accounts
    private void LoadUsers()
    {
        if (!File.Exists(FilePath))
            return;

        string json = File.ReadAllText(FilePath);

        Users = JsonSerializer.Deserialize<List<User>>(json)
                ?? new List<User>();
    }

    // Hide password while typing
    private string ReadPassword()
    {
        string password = "";

        while (true)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password += key.KeyChar;
                Console.Write("*");
            }
        }

        return password;
    }

    // Password hashing
    private string HashPassword(string password, byte[] salt)
    {
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100000,
            HashAlgorithmName.SHA256,
            32);

        return Convert.ToBase64String(hash);
    }

    // Registration
    public void Register()
    {
        Console.Write("Full name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            Users.Any(u => u.Email.Equals(
                email, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("Invalid information or email already exists!");
            return;
        }

        Console.Write("Password: ");
        string password = ReadPassword();

        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Password cannot be empty!");
            return;
        }

        byte[] salt = RandomNumberGenerator.GetBytes(16);

        string saltString = Convert.ToBase64String(salt);
        string hash = HashPassword(password, salt);

        var newUser = new User(name, email, hash, saltString);

        Users.Add(newUser);
        CurrentUser = newUser;

        SaveUsers();

        Console.WriteLine("Registration successful!");
        Console.WriteLine($"Welcome, {name}!");
    }

    // Login
    public void Login()
    {
        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";

        Console.Write("Password: ");
        string password = ReadPassword();

        var user = Users.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            Console.WriteLine("Account not found!");
            return;
        }

        byte[] salt = Convert.FromBase64String(user.PasswordSalt);
        string hash = HashPassword(password, salt);

        bool correct = CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(hash),
            Convert.FromBase64String(user.PasswordHash));

        if (correct)
        {
            CurrentUser = user;
            Console.WriteLine($"Welcome back, {user.FullName}!");
        }
        else
        {
            Console.WriteLine("Incorrect password!");
        }
    }

    // Logout
    public void Logout()
    {
        CurrentUser = null;
        Console.WriteLine("You have logged out!");
    }
}