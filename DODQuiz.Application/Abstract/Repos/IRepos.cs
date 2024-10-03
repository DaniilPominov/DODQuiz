using CSharpFunctionalExtensions;
using ErrorOr;

namespace DODQuiz.Application.Abstract.Repos
{
    public interface IRepos<TEntity> where TEntity : Entity<Guid>
    {
        Task<ErrorOr<List<TEntity>>> GetAllAsync(CancellationToken cancellationToken);
        Task<ErrorOr<TEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<ErrorOr<Success>> AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task<ErrorOr<Success>> DeleteAsync(TEntity entity, CancellationToken cancellationToken);
        Task<ErrorOr<Success>> UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        Task<ErrorOr<List<TEntity>>> GetManyByIdAsync(List<Guid> listOfId, CancellationToken cancellationToken);
        Task<ErrorOr<Success>> Attach(TEntity entity, CancellationToken cancellationToken);
    }
}
