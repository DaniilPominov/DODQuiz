using DODQuiz.Core.Entyties;
using ErrorOr;

namespace DODQuiz.Application.Abstract.Repos
{
    public interface IUserRepos : IRepos<User>
    {
        Task<ErrorOr<User>> GetUserByName(string name, CancellationToken cancellationToken);
    }
}
