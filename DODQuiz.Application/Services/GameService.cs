using DODQuiz.Application.Abstract.Repos;
using DODQuiz.Application.Abstract.Services;
using DODQuiz.Contracts;
using DODQuiz.Core.Entyties;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DODQuiz.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IUserRepos _userRepository;
        private readonly IQuestionRepos _questionRepository;
        public GameService(IUserRepos userRepository, IQuestionRepos questionRepository)
        {
            _userRepository = userRepository;
            _questionRepository = questionRepository;
        }
        public async Task<ErrorOr<List<Question>>> GetAllQuestions(CancellationToken cancellationToken)
        {
            return await _questionRepository.GetAllAsync(cancellationToken);
        }
        public async Task<ErrorOr<Success>> AddQuestion(QuestionRequest questionRequest,CancellationToken cancellationToken)
        {
            var question = Question.Create(
                Guid.NewGuid(),
                questionRequest.name,
                questionRequest.description,
                questionRequest.category,
                questionRequest?.imageUri);
            return await _questionRepository.AddAsync(question.Value,cancellationToken);
        }

        public async Task<ErrorOr<Success>> DeleteQuestion(Guid questionId, CancellationToken cancellationToken)
        {
            var result = await _questionRepository.DeleteAsync(questionId, cancellationToken);
            if (result.IsError)
            {
                return Error.Failure();
            }
            return result;
        }

        public Task<ErrorOr<Success>> UpdateQuestion(Guid id, QuestionRequest questionRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
