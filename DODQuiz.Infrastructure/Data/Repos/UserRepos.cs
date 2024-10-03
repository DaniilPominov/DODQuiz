using DODQuiz.Application.Abstract.Repos;
using DODQuiz.Core.Entyties;
using DODQuiz.Infrastructure.Data.Context;
using ErrorOr;

namespace DODQuiz.Infrastructure.Data.Repos
{
    public class UserRepos : BaseRepository<User>,IPlayerRepos

    {
        public UserRepos(GameContext gameContext) : base(gameContext) { }
        
    }
}
