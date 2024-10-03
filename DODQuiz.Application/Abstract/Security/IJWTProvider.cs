using DODQuiz.Core.Entyties;

namespace DODQuiz.Application.Abstract.Security
{
    public interface IJWTProvider
    {
        string GenerateToken(User user);
    }
}
