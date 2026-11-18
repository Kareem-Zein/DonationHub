using Domain.Abstractions;

namespace Application.Abstractions.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<Result> ExecuteInTransactionAsync(Func<CancellationToken, Task<Result>> func, CancellationToken cancellationToken);
    Task<Result<T>> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<Result<T>>> func, CancellationToken cancellationToken);
}
