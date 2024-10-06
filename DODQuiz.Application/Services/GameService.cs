using DODQuiz.Application.Abstract.Repos;
using DODQuiz.Application.Abstract.Services;
using DODQuiz.Contracts;
using DODQuiz.Core.Entyties;
using System.Collections.Concurrent;
using ErrorOr;
using Microsoft.Extensions.Configuration;

namespace DODQuiz.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IUserRepos _userRepository;
        private readonly IQuestionRepos _questionRepository;
        private readonly IConfiguration _configuration;
        //idn have enough time to fix it
        private static List<Question> _questions = new();
        private static List<User> _users = new();
        private static ConcurrentDictionary<User, Question> _userToQuestion = new();
        private static ConcurrentDictionary<User, string> _userToCategory = new();
        private static ConcurrentDictionary<string, List<Guid>> _recentQuestions = new();
        private static ConcurrentDictionary<Guid,bool> _userStatuses = new();
        private const int _recentDepth = 2;//5 in release
        private string _rootCode = "";
        public GameService(IUserRepos userRepository, IQuestionRepos questionRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _questionRepository = questionRepository;
            _questions = _questionRepository.GetAllAsync(CancellationToken.None).Result.Value;
            _configuration = configuration;
            _rootCode = _configuration?.GetRequiredSection("QuizOptions:RootCode").Value ?? "bober123";
        }
        public async Task<ErrorOr<ConcurrentDictionary<User, Question>>> GetUserToQuestion(CancellationToken cancellationToken)
        {
            return _userToQuestion;
        }
        public async Task<ErrorOr<List<User>>> GetAllUsers(CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetAllAsync(cancellationToken);
            return result;
        }
        public async Task<ErrorOr<Success>> ChangeUserStatus(Guid id, string code, CancellationToken cancellationToken)
        {
            var user = _users.Find(x => x.Id == id);
            if (user == null)
            {
                return Error.NotFound();
            }
            if (!_userStatuses.ContainsKey(id))
            {
                _userStatuses.TryAdd(id, false);
            }
            if (code != _rootCode)
            {
                return Error.Forbidden();
            }
            _userStatuses[id] = !_userStatuses[id];
            return Result.Success;
        }
        public async Task<ErrorOr<ConcurrentDictionary<Guid, bool>>> GetUsersStatuses(CancellationToken cancellationToken)
        {
            return _userStatuses;
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
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user.IsError)
            {
                return Error.NotFound();
            }
            if (!_users.Contains(user.Value))
            {
                await AddUserToGame(userId, cancellationToken);
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
            categories = _questions.Select(q => q.Category).Distinct().ToList();
            return categories;

        }
        public async Task<ErrorOr<Success>> AddQuestion(QuestionRequest questionRequest, CancellationToken cancellationToken)
        {
            var question = Question.Create(
                Guid.NewGuid(),
                questionRequest.name,
                questionRequest.description,
                questionRequest.category,
                questionRequest?.imageUri);
            return await _questionRepository.AddAsync(question.Value, cancellationToken);
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

        public async Task<ErrorOr<Success>> StartRound(CancellationToken cancellationToken)
        {

            return await GenerateQuesitons(cancellationToken);
        }
        private async Task<ErrorOr<Success>> GenerateQuesitons(CancellationToken cancellationToken)
        {

            if (_questions.Count == 0)
            {
                var questions = await _questionRepository.GetAllAsync(cancellationToken);
                _questions = questions.Value;
            }
            foreach (var user in _userToCategory.Keys)
            {
                var categoryname = _userToCategory[user];
                var category = _questions.Where(q => q.Category == categoryname).ToList();
                var random = new Random();
                var newquestion = category[random.Next(category.Count)];
                if (!_recentQuestions.Keys.Contains(categoryname))
                {
                    _recentQuestions[categoryname] = new List<Guid>() { newquestion.Id };
                }
                while (_recentQuestions[categoryname].Contains(newquestion.Id))
                {
                    newquestion = category[random.Next(category.Count)];
                }
                if (_recentQuestions[categoryname].Count >= _recentDepth)
                {
                    _recentQuestions[categoryname].RemoveAt(_recentDepth - 1);
                }
                _recentQuestions[categoryname].Add(newquestion.Id);

                _userToQuestion[user] = newquestion;
            }
            return Result.Success;
        }
    }
}
