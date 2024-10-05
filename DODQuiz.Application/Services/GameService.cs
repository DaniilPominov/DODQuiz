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
        private List<Question> _questions = new();
        private List<User> _users = new();
        private Dictionary<User,Question> _userToQuestion = new();
        private Dictionary<User,string> _userToCategory = new();
        public GameService(IUserRepos userRepository, IQuestionRepos questionRepository)
        {
            _userRepository = userRepository;
            _questionRepository = questionRepository;
            _questions = _questionRepository.GetAllAsync(CancellationToken.None).Result.Value;
        }
        public async Task<ErrorOr<List<User>>> GetAllUsers(CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetAllAsync(cancellationToken);
            return result;
        }
        public async Task<ErrorOr<Success>> AddUserToGame(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            _users.Add(user.Value);
            return Result.Success;
        }
        public async Task<ErrorOr<Success>> RemoveUserFromGame(Guid userId, CancellationToken cancellationToken)
        {
            if (_users.Any(u => u.Id == userId))
            {
                var user = _users.FirstOrDefault(u => u.Id == userId);
                _users.Remove(user);
                return Result.Success;
            }
            return Error.NotFound();
        }
        public async Task<ErrorOr<Success>> ChangeUserQuestion(Guid userId, Guid questionId, CancellationToken cancellationToken)
        {
            var user = _userToQuestion.Keys.ToList().Find(u => u.Id == userId);
            var question = _questions.Find(u => u.Id == questionId);
            if (user == null)
            {
                return Error.NotFound();
            }
            if (question == null)
            {
                return Error.Validation();
            }
            _userToQuestion[user] = question;
            return Result.Success;

        }
        public async Task<ErrorOr<Success>> ChangeUserQuestionCategory(Guid userId, string categoryName, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId,cancellationToken);
            if (user.IsError) 
            {
                return Error.NotFound();
            }
            _userToCategory[user.Value] = categoryName;
            return Result.Success;
        }
        public async Task<ErrorOr<List<Question>>> GetAllQuestions(CancellationToken cancellationToken)
        {
            return await _questionRepository.GetAllAsync(cancellationToken);
        }
        public async Task<ErrorOr<List<string>>> GetQuestionsCategories(CancellationToken cancellationToken)
        {
            List<string> categories = new List<string>();
            categories = _questions.Select(q=> q.Category).Distinct().ToList();
            return categories;

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

        public async Task<ErrorOr<List<User>>> GetAllInGameUsers(CancellationToken cancellationToken)
        {
            return _users;
        }
    }
}
