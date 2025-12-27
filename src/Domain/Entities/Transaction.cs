namespace Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    // Foreign Key
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public static Transaction Create(string description, decimal amount, int categoryId)
    {
        // Here you can add business rules, e.g., validation that belongs to the domain
        return new Transaction
        {
            Description = description,
            Amount = amount,
            CategoryId = categoryId,
            Date = DateTime.UtcNow
        };
    }
}