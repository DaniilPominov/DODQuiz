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
    internal class RoleRepos : IRoleRepos
    {
        public Task<ErrorOr<Success>> AddAsync(Role entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<Success>> DeleteAsync(Role entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<List<Role>>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<Role>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<List<Role>>> GetManyByIdAsync(List<Guid> listOfId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorOr<Success>> UpdateAsync(Role entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
