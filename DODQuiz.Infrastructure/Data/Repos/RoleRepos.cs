using DODQuiz.Application.Abstract.Repos;
using DODQuiz.Core.Entyties;
using DODQuiz.Infrastructure.Data.Context;
using ErrorOr;

namespace DODQuiz.Infrastructure.Data.Repos
{
    public class RoleRepos : BaseRepository<Role>, IRoleRepos
    {
        public RoleRepos(GameContext gameContext) : base(gameContext) { }
    }
}
