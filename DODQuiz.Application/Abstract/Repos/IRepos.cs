using CSharpFunctionalExtensions;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
