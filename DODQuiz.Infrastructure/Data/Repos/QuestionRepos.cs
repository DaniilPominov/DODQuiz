using DODQuiz.Application.Abstract.Repos;
using DODQuiz.Core.Entyties;
using DODQuiz.Infrastructure.Data.Context;

namespace DODQuiz.Infrastructure.Data.Repos
{
    public class QuestionRepos : BaseRepository<Question>, IQuestionRepos
    {
        public QuestionRepos(GameContext gameContext) : base(gameContext) { }
    }
}
