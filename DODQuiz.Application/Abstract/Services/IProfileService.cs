using DODQuiz.Contracts;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DODQuiz.Application.Abstract.Services
{
    public interface IProfileService
    {
        Task<ErrorOr<string>> Login(string Email, string Password);
        Task<ErrorOr<Success>> Register(UserRegisterRequest userRegistrationRequest,CancellationToken cancellationToken);
    }
}
