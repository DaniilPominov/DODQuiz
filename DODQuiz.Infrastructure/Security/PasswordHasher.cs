using DODQuiz.Application.Abstract.Security;

namespace DODQuiz.Infrastructure.Security
{
    internal class PasswordHasher : IPasswordHasher
    {
        public string Generate(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool VerifyPassword(string password, string HashedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, HashedPassword);
        }
    }
}
