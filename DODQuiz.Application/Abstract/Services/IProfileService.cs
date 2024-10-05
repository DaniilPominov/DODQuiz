using DODQuiz.Contracts;
using ErrorOr;

namespace DODQuiz.Application.Abstract.Services
{
    public interface IProfileService
    {
        Task<ErrorOr<string>> Login(string Email, string Password);
        Task<ErrorOr<Success>> Register(UserRegisterRequest userRegistrationRequest, CancellationToken cancellationToken);
    }
}
