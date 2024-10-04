using DODQuiz.Contracts;
using DODQuiz.Core.Entyties;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DODQuiz.Application.Abstract.Services
{
    public interface IGameService
    {
        Task<ErrorOr<List<Question>>> GetAllQuestions(CancellationToken cancellationToken);
        Task<ErrorOr<Success>> AddQuestion(QuestionRequest questionRequest, CancellationToken cancellationToken);
        Task<ErrorOr<Success>> DeleteQuestion(Guid questionId, CancellationToken cancellationToken);
        Task<ErrorOr<Success>> UpdateQuestion(Guid id, QuestionRequest questionRequest, CancellationToken cancellationToken);
        Task<ErrorOr<List<User>>> GetAllUsers(CancellationToken cancellationToken);
        Task<ErrorOr<List<User>>> GetAllInGameUsers(CancellationToken cancellationToken);
        Task<ErrorOr<Success>> AddUserToGame(Guid userId, CancellationToken cancellationToken);
        Task<ErrorOr<Success>> RemoveUserFromGame(Guid userId, CancellationToken cancellationToken);
        Task<ErrorOr<Success>> ChangeUserQuestion(Guid userId, Guid questionId, CancellationToken cancellationToken);


    }
}
