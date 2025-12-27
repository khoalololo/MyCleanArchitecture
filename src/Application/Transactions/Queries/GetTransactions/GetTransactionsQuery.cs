using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Transactions.Queries.GetTransactions;

public record GetTransactionsQuery : IRequest<List<TransactionResponse>>;

public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, List<TransactionResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetTransactionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<TransactionResponse>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = _context.Transactions
            .AsEnumerable() // Move to memory to avoid EF Core dependency in Application layer for Select
            .Select(t => new TransactionResponse(
                t.Id,
                t.Description,
                t.Amount,
                t.Date,
                t.CategoryId,
                t.Category != null ? t.Category.Name : "Unknown"
            ))
            .ToList();

        return Task.FromResult(transactions);
    }
}
