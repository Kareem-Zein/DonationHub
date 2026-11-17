using Application.Abstractions.Interfaces;
using Domain.Abstractions;
using Infrastructure.Data;

namespace Infrastructure.Implementations;

internal class UnitOfWork(AppDbContext appDbContext) : IUnitOfWork
{
    public async Task<Result> ExecuteInTransactionAsync(Func<CancellationToken, Task<Result>> func, CancellationToken cancellationToken)
    {
        await using var transaction = await appDbContext.Database.BeginTransactionAsync(cancellationToken);

        var result = await func(cancellationToken);

        if (result.IsSuccess)
            await transaction.CommitAsync(cancellationToken);

        return result;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => await appDbContext.SaveChangesAsync(cancellationToken);
}
