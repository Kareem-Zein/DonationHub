using Domain.Entities.Base;
using System.Linq.Expressions;

namespace Application.Abstractions.Interfaces.Repositories;

public interface IReadOnlyRepository<T> where T: EntityBase
{
    Task<IReadOnlyList<TSelector>> ListAsync<TSelector>(Expression<Func<T, TSelector>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    Task<IReadOnlyList<TSelector>> ListAsync<TSelector>(Expression<Func<T, TSelector>> selector, CancellationToken cancellationToken);
    Task<TSelector?> FirstOrDefaultAsync<TSelector>(Expression<Func<T, TSelector>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    Task<TSelector?> FirstOrDefaultAsync<TSelector>(Expression<Func<T, TSelector>> selector, CancellationToken cancellationToken);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    Task<bool> AnyAsync(CancellationToken cancellationToken);
}
