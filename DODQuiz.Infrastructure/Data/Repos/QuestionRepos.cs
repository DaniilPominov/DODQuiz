using DODQuiz.Application.Abstract.Repos;
using DODQuiz.Core.Entyties;
using ErrorOr;

namespace DODQuiz.Infrastructure.Data.Repos
{
    internal class QuestionRepos : IQuestionRepos
    {
        public Task<ErrorOr<Success>> AddAsync(Question entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<Success>> DeleteAsync(Question entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<List<Question>>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<Question>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<List<Question>>> GetManyByIdAsync(List<Guid> listOfId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<Success>> UpdateAsync(Question entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
