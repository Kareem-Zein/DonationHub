using Domain.Abstractions;

namespace Application.Abstractions.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<Result> ExecuteInTransactionAsync(Func<CancellationToken, Task<Result>> func, CancellationToken cancellationToken);
}
