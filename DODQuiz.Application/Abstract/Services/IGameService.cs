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


    }
}
