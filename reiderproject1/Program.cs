using reiderproject1.Models;
using reiderproject1.Services;

class Program
{
    static AccountService account = new();
    static ShopService shop = new();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n================================");
            Console.WriteLine("       WELCOME TO OUR SHOP");
            Console.WriteLine("        YOUR DAILY MARKET");
            Console.WriteLine("================================");

            Console.WriteLine("\n1. Shop");
            Console.WriteLine("2. Cart");
            Console.WriteLine("3. Profile");
            Console.WriteLine("4. More Information");

            if (account.CurrentUser != null)
                Console.WriteLine("5. Log Out");

            Console.WriteLine(account.CurrentUser == null
                ? "5. Exit"
                : "6. Exit");

            Console.Write("\nChoose: ");
            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    OpenShop();
                    break;

                case "2":
                    if (account.CurrentUser == null)
                        Console.WriteLine("Please log in first!");
                    else
                        shop.ShowCart(account.CurrentUser);
                    break;

                case "3":
                    openProfile();
                    break;

                case "4":
                    Console.WriteLine("\nYOUR DAILY MARKET");
                    Console.WriteLine("Welcome to our online shop!");
                    Console.WriteLine("Find your favorite products here.");
                    break;

                case "5":
                    if (account.CurrentUser != null)
                        account.Logout();
                    else
                        return;
                    break;

                case "6":
                    if (account.CurrentUser != null)
                        return;
                    break;

                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        }
    }

    static void OpenShop()
    {
        shop.ShowCategories();

        Console.Write("\nChoose category number: ");

        if (!int.TryParse(Console.ReadLine(), out int choice))
            return;

        var categories = shop.Products
            .Select(p => p.Category)
            .Distinct()
            .ToList();

        if (choice < 1 || choice > categories.Count)
        {
            Console.WriteLine("Invalid category!");
            return;
        }

        shop.ShowProducts(categories[choice - 1]);

        Console.Write("\nEnter product ID to add (0 to return): ");

        if (!int.TryParse(Console.ReadLine(), out int id) || id == 0)
            return;

        var product = shop.Products.FirstOrDefault(p =>
            p.Id == id && p.Category == categories[choice - 1]);

        if (product == null)
        {
            Console.WriteLine("Product not found!");
            return;
        }

        if (account.CurrentUser == null)
        {
            Console.WriteLine("\nYou must log in first!");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Return");

            string option = Console.ReadLine() ?? "";

            if (option == "1")
            {
                account.Login();
            }
            else if (option == "2")
            {
                account.Register();
            }
            else
            {
                return;
            }

            if (account.CurrentUser == null)
            {
                Console.WriteLine("Authentication failed!");
                return;
            }
        }

        Console.WriteLine($"\nSelected product: {product.Name}");
        
        shop.AddToCart(account.CurrentUser, product);
    }

    static void OpenProfile()
    {
        if (account.CurrentUser != null)
        {
            Console.WriteLine("\n--- YOUR PROFILE ---");
            Console.WriteLine($"Name: {account.CurrentUser.FullName}");
            Console.WriteLine($"Email: {account.CurrentUser.Email}");
            return;
        }

        Console.WriteLine("\n--- PROFILE ---");
        Console.WriteLine("1. Login");
        Console.WriteLine("2. Register");
        Console.WriteLine("3. Return");

        string choice = Console.ReadLine() ?? "";

        switch (choice)
        {
            case "1":
                account.Login();
                break;

            case "2":
                account.Register();
                break;
        }
    }
}