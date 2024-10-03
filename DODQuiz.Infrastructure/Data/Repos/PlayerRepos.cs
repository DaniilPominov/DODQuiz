using DODQuiz.Application.Abstract.Repos;
using DODQuiz.Core.Entyties;
using ErrorOr;

namespace DODQuiz.Infrastructure.Data.Repos
{
    internal class PlayerRepos : IPlayerRepos
    {
        public Task<ErrorOr<Success>> AddAsync(User entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<Success>> DeleteAsync(User entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<List<User>>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<List<User>>> GetManyByIdAsync(List<Guid> listOfId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<Success>> UpdateAsync(User entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
