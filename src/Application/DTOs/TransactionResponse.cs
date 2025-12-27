namespace Application.DTOs;

public record TransactionResponse(
    int Id,
    string Name,
    decimal Amount,
    DateTime Date,
    int CategoryId,
    string CategoryName
);
