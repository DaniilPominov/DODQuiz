using DODQuiz.Application.Abstract.Repos;
using DODQuiz.Core.Entyties;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
