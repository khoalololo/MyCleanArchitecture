using Domain.Entities;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    IQueryable<Category> Categories { get; }
    IQueryable<Transaction> Transactions { get; }
    IQueryable<User> Users { get; }
    
    void Add<TEntity>(TEntity entity) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
