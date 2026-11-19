namespace reiderproject1.Services;
using reiderproject1.Models;

public class ShopService
{
    public List<Product> Products = new()
    {
        new Product(1, "Burger", 8.50, "Fast Food"),
        new Product(2, "Pizza", 12.00, "Fast Food"),
        new Product(3, "Hot Dog", 6.00, "Fast Food"),

        new Product(4, "White Bread", 2.00, "Bakery"),
        new Product(5, "Croissant", 3.50, "Bakery"),

        new Product(6, "Water", 1.00, "Drinks"),
        new Product(7, "Juice", 3.00, "Drinks"),

        new Product(8, "Apple", 1.50, "Fruits"),
        new Product(9, "Banana", 2.00, "Fruits"),

        new Product(10, "Chocolate", 4.00, "Sweets"),
        new Product(11, "Candy", 2.50, "Sweets"),

        new Product(12, "Milk", 3.00, "Dairy"),
        new Product(13, "Cheese", 7.00, "Dairy")
    };

    public void ShowCategories()
    {
        Console.WriteLine("\n--- SHOP CATEGORIES ---");

        var categories = Products
            .Select(p => p.Category)
            .Distinct()
            .ToList();

        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {categories[i]}");
        }
    }

    public void ShowProducts(string category)
    {
        var products = Products
            .Where(p => p.Category == category)
            .ToList();

        Console.WriteLine($"\n--- {category} ---");

        foreach (var product in products)
        {
            Console.WriteLine(
                $"{product.Id}. {product.Name} - {product.Price:F2} GEL");
        }
    }

    public void AddToCart(User user, Product product)
    {
        Console.Write("Quantity: ");

        if (!int.TryParse(Console.ReadLine(), out int quantity) ||
            quantity <= 0)
        {
            Console.WriteLine("Invalid quantity!");
            return;
        }

        var item = user.Cart.FirstOrDefault(
            c => c.Product.Id == product.Id);

        if (item != null)
            item.Quantity += quantity;
        else
            user.Cart.Add(new CartItem(product, quantity));

        Console.WriteLine(
            $"{product.Name} x{quantity} added to your cart!");
    }

    public void ShowCart(User user)
    {
        Console.WriteLine("\n--- YOUR CART ---");

        if (user.Cart.Count == 0)
        {
            Console.WriteLine("Your cart is empty!");
            return;
        }

        double total = 0;

        foreach (var item in user.Cart)
        {
            double price = item.Product.Price * item.Quantity;
            total += price;

            Console.WriteLine(
                $"{item.Product.Name} x{item.Quantity} = {price:F2} GEL");
        }

        Console.WriteLine($"TOTAL: {total:F2} GEL");
    }
}