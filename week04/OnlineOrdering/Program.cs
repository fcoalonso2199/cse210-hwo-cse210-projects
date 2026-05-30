using System;
using System.Collections.Generic;

public class Address
{
    private string _street;
    private string _city;
    private string _state;
    private string _country;

    public Address(string street, string city, string state, string country)
    {
        _street = street;
        _city = city;
        _state = state;
        _country = country;
    }

    public bool IsInUSA()
    {
        return _country.ToLower() == "usa" || _country.ToLower() == "united states";
    }

    public string GetFullAddress()
    {
        return $"{_street}\n{_city}, {_state}\n{_country}";
    }
}

public class Customer
{
    private string _name;
    private Address _address;

    public Customer(string name, Address address)
    {
        _name = name;
        _address = address;
    }

    public string GetName() => _name;

    public bool LivesInUSA()
    {
        return _address.IsInUSA();
    }

    public string GetAddressString()
    {
        return _address.GetFullAddress();
    }
}

public class Product
{
    private string _name;
    private string _productId;
    private decimal _price;
    private int _quantity;

    public Product(string name, string productId, decimal price, int quantity)
    {
        _name = name;
        _productId = productId;
        _price = price;
        _quantity = quantity;
    }

    public decimal GetTotalCost() => _price * _quantity;
    public string GetName() => _name;
    public string GetId() => _productId;
}

public class Order
{
    private List<Product> _products;
    private Customer _customer;

    public Order(Customer customer)
    {
        _customer = customer;
        _products = new List<Product>();
    }

    public void AddProduct(Product product)
    {
        _products.Add(product);
    }

    public decimal CalculateTotal()
    {
        decimal total = 0;
        foreach (var product in _products)
        {
            total += product.GetTotalCost();
        }

        total += _customer.LivesInUSA() ? 5 : 35;
        return total;
    }

    public string GetPackingLabel()
    {
        string label = "--- Packing Label ---\n";
        foreach (var product in _products)
        {
            label += $"Product: {product.GetName()} (ID: {product.GetId()})\n";
        }
        return label;
    }

    public string GetShippingLabel()
    {
        return $"--- Shipping Label ---\nCustomer: {_customer.GetName()}\nAddress:\n{_customer.GetAddressString()}\n";
    }
}

class Program
{
    static void Main(string[] args)
    {
        Address addr1 = new Address("123 Hurraicane St", "St George", "Utah", "USA");
        Customer cust1 = new Customer("Luis Enrique", addr1);
        Order order1 = new Order(cust1);
        order1.AddProduct(new Product("Laptop", "LPT-01", 800m, 1));
        order1.AddProduct(new Product("Mouse", "MOU-01", 20m, 2));

        Address addr2 = new Address("109 Aldolfo Lopez", "Mexico City", "DF", "Mexico");
        Customer cust2 = new Customer("President Ruiz", addr2);
        Order order2 = new Order(cust2);
        order2.AddProduct(new Product("Monitor", "MON-01", 150m, 1));
        order2.AddProduct(new Product("Keyboard", "KEY-01", 45m, 1));
        order2.AddProduct(new Product("HDMI Cable", "CAB-01", 10m, 3));

        DisplayOrder(order1);
        DisplayOrder(order2);
    }

    static void DisplayOrder(Order order)
    {
        Console.WriteLine(order.GetPackingLabel());
        Console.WriteLine(order.GetShippingLabel());
        Console.WriteLine($"Total Price: ${order.CalculateTotal():F2}");
        Console.WriteLine(new string('-', 30) + "\n");
    }
}