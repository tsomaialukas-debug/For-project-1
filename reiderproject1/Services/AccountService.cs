using reiderproject1.Models;
using System.Security.Cryptography;
using System.Text.Json;

namespace reiderproject1.Services;

public class AccountService
{
    private readonly string dataFolder;
    private readonly string usersFile;

    public List<User> Users { get; private set; } = new();

    public User? CurrentUser { get; private set; }

    public AccountService()
    {
        dataFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data");

        usersFile = Path.Combine(dataFolder, "users.json");

        Directory.CreateDirectory(dataFolder);

        LoadUsers();
    }

    // SAVE USERS
    private void SaveUsers()
    {
        string json = JsonSerializer.Serialize(
            Users,
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(usersFile, json);
    }

    // LOAD USERS
    private void LoadUsers()
    {
        if (File.Exists(usersFile))
        {
            string json = File.ReadAllText(usersFile);

            Users = JsonSerializer.Deserialize<List<User>>(json)
                    ?? new List<User>();
        }
    }

    // PASSWORD HASHING
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

    // SIGN UP
    public void SignUp()
    {
        Console.Write("Full name: ");
        string fullName = Console.ReadLine() ?? "";

        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";

        Console.Write("Password: ");
        string password = ReadPassword();

        if (string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("All fields are required!");
            return;
        }

        if (Users.Any(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("This email is already registered!");
            return;
        }

        byte[] salt = RandomNumberGenerator.GetBytes(16);

        string hash = HashPassword(password, salt);

        User user = new User
        {
            FullName = fullName,
            Email = email,
            PasswordHash = hash,
            PasswordSalt = Convert.ToBase64String(salt)
        };

        Users.Add(user);
        SaveUsers();

        Console.WriteLine("\nAccount created successfully!");
        Console.WriteLine("Now please log in.\n");

        Login();
    }

    // LOG IN
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

    // HIDDEN PASSWORD INPUT
    private string ReadPassword()
    {
        string password = "";

        ConsoleKeyInfo key;

        while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
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

        Console.WriteLine();

        return password;
    }

    public void Logout()
    {
        CurrentUser = null;
        Console.WriteLine("Logged out successfully!");
    }
}