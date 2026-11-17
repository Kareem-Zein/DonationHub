using Domain.Interfaces.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Data.Interseptors;

internal sealed class SoftDeletableInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    private readonly TimeProvider _timeProvider = timeProvider;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            SetSoftDeleteableData(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            SetSoftDeleteableData(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetSoftDeleteableData(DbContext context)
    {
        var utcNow = _timeProvider.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletableEntity>())
        {
            if (entry.State != EntityState.Deleted)
                continue;

            if (entry.Entity.IsDeleted)
            {
                entry.State = EntityState.Unchanged;
                continue;
            }

            entry.State = EntityState.Unchanged;
            entry.Entity.Delete(utcNow);

            entry.Property(nameof(ISoftDeletableEntity.IsDeleted))
                .IsModified = true;

            entry.Property(nameof(ISoftDeletableEntity.DeletedAtUtc))
                .IsModified = true;

            entry.Property(nameof(ISoftDeletableEntity.RestoredAtUtc))
                .IsModified = true;
        }
    }
}
