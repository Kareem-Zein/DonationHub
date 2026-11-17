using Domain.Entities.Base;

namespace Application.Abstractions.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : EntityBase
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task AddAsync(T entity, CancellationToken cancellationToken);

        void Update(T entity);
        void Remove(T entity);
    }
}
