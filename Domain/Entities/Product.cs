namespace Domain.Entities;

public sealed class Product(string name, string? description, decimal price) : DomainEntity
{
    public string Name { get; set; } = name;
    public string? Description { get; set; } = description;
    public decimal Price { get; set; } = price;
    public bool Active { get; set; } = true;
}
