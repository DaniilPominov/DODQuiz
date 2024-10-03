namespace DODQuiz.Application.Abstract.Security
{
    public interface IPasswordHasher
    {
        string Generate(string password);

        bool VerifyPassword(string password, string HashedPassword);
    }
}
