using DODQuiz.Application.Abstract.Services;
using DODQuiz.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DODQuiz.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "admin")]
    [ApiController]
    public class AdminController : ControllerBase

    {

        private readonly IProfileService profileService;
        private readonly IGameService gameService;
        public AdminController(IProfileService profileService, IGameService gameService)
        {
            this.profileService = profileService;
            this.gameService = gameService;
        }
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(UserRegisterRequest registerRequest, CancellationToken cancellationToken)
        {
            var result = await profileService.Register(registerRequest, cancellationToken);
            return Ok(result);
        }
        [HttpPost("SetUserCategory")]
        public async Task<ActionResult> SetUserCategory(Guid userId, string categoryName, CancellationToken cancellationToken)
        {
            var result = await gameService.ChangeUserQuestionCategory(userId, categoryName, cancellationToken);
            if (result.IsError)
            {
                return BadRequest(result);
            }
            return Ok(result.Value);
        }
        [HttpGet("GetUsers")]
        public async Task<ActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var users = await gameService.GetAllUsers(cancellationToken);
            if (users.IsError)
            {
                return BadRequest(users);
            }
            var result = new List<UserResponse>();
            foreach (var user in users.Value)
            {
                result.Add(new UserResponse(user.Id, user.Name));
            }
            return Ok(result);
        }
        [HttpGet("GetInGameUsers")]
        public async Task<ActionResult> GetInGameUsers(CancellationToken cancellationToken)
        {
            var result = await gameService.GetAllInGameUsers(cancellationToken);
            if (result.IsError)
            {
                return BadRequest(result);
            }
            return Ok(result.Value);
        }

        [HttpGet("GetAllQuestions")]
        public async Task<ActionResult> GetAllQuestions(CancellationToken cancellationToken)
        {
            var result = await gameService.GetAllQuestions(cancellationToken);
            if (result.IsError)
            {
                return BadRequest(result);
            }
            return Ok(result.Value);
        }

        [HttpPost("AddQuestion")]
        public async Task<ActionResult> AddQuestion(QuestionRequest questionRequest, CancellationToken cancellationToken)
        {
            var result = await gameService.AddQuestion(questionRequest, cancellationToken);
            if (result.IsError)
            {
                return BadRequest(result);
            }
            await gameService.UpdateQuestions(cancellationToken);
            return Ok(result);
        }
        [HttpPut("EditQuestion")]
        public async Task<ActionResult> EditQuestion(Guid id, QuestionRequest questionRequest, CancellationToken cancellationToken)
        {
            return Ok();
        }
        [HttpDelete("DeleteQuestion")]
        public async Task<ActionResult> DeleteQuestion(Guid id, CancellationToken cancellationToken)
        {
            var result = await gameService.DeleteQuestion(id, cancellationToken);
            if (result.IsError)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}
