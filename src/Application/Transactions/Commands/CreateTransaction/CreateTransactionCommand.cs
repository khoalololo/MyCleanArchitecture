using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Transactions.Commands.CreateTransaction;

public record CreateTransactionCommand(string Name, decimal Amount, int CategoryId) : IRequest<int>;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, int>
{
    private readonly IApplicationDbContext _context;

    // 6. DI creates the handler and injects ITS dependencies
    public CreateTransactionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<int> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = Transaction.Create(request.Name, request.Amount, request.CategoryId);

        _context.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}
