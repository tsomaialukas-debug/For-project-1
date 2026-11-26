using reiderproject1.Models;

using reiderproject1.Services;
using Spectre.Console;
using System;
using System.Linq;

class Program
{
    static AccountService account = new();
    static ShopService shop = new();

    static void Main()
    {
        while (true)
        {
            if (account.CurrentUser == null)
            {
                string guestChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]WELCOME TO OUR SHOP[/]")
                        .AddChoices("Shop", "Login", "Register", "More Information", "Exit"));

                Console.Clear();

                switch (guestChoice)
                {
                    case "Shop":
                        OpenShop();
                        break;

                    case "Login":
                        account.Login();
                        break;

                    case "Register":
                        account.SignUp();
                        break;

                    case "More Information":
                        Console.WriteLine("Welcome to our online shop!");
                        break;

                    case "Exit":
                        return;
                }
            }
            else
            {
                string userChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]WELCOME BACK TO OUR SHOP[/]")
                        .AddChoices("Shop", "Cart", "Profile", "More Information", "Log Out", "Exit"));

                Console.Clear();

                switch (userChoice)
                {
                    case "Shop":
                        OpenShop();
                        break;

                    case "Cart":
                        shop.ShowCart(account.CurrentUser);
                        break;

                    case "Profile":
                        OpenProfile();
                        break;

                    case "More Information":
                        Console.WriteLine("Welcome to our online shop!");
                        break;

                    case "Log Out":
                        account.Logout();
                        break;

                    case "Exit":
                        return;
                }
            }
        }
    }

    static void OpenShop()
    {
        var categories = shop.Products
            .Select(product => product.Category)
            .Distinct()
            .ToList();

        Console.Clear();
        string category = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a category")
                .AddChoices(categories));

        var products = shop.Products
            .Where(product => product.Category == category)
            .ToList();

        var productChoices = products.ToDictionary(
            product => $"{product.Name} - [green]{product.Price:F2} GEL[/]",
            product => product);

        string selectedProduct = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Select a product from [yellow]{category}[/]")
                .AddChoices(productChoices.Keys));

        Product? product = productChoices[selectedProduct];

        if (product == null)
        {
            Console.WriteLine("Product not found!");
            return;
        }

        if (account.CurrentUser == null)
        {
            Console.WriteLine("Please log in to add products to your cart.");
            return;
        }

        shop.AddToCart(account.CurrentUser, product);
    }

    static void OpenProfile()
    {
        if (account.CurrentUser == null)
            return;

        Console.WriteLine("\n--- PROFILE ---");
        Console.WriteLine($"Name: {account.CurrentUser.FullName}");
        Console.WriteLine($"Email: {account.CurrentUser.Email}");
        Console.WriteLine($"Cart items: {account.CurrentUser.Cart.Sum(item => item.Quantity)}");
    }
}