using DODQuiz.Application.Abstract.Repos;
using DODQuiz.Core.Entyties;
using DODQuiz.Infrastructure.Data.Context;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace DODQuiz.Infrastructure.Data.Repos
{
    public class UserRepos : BaseRepository<User>, IUserRepos

    {
        public UserRepos(GameContext gameContext) : base(gameContext) { }

        public async Task<ErrorOr<User>> GetUserByName(string name, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.users.Where(u => u.Name == name).Include(u => u.Roles).FirstOrDefaultAsync(cancellationToken);

                if (user is null)
                {
                    return Error.NotFound(description: $"User with name: {name} not found");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + $" can't found User with name: {name}");
            }
        }
    }
}
