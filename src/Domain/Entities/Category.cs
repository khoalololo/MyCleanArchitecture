namespace Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Relationship: One to Many
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}