using DODQuiz.Application.Abstract.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DODQuiz.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy ="user")]
    [ApiController]
    public class GameController : ControllerBase
    {
        IGameService gameService;
        public GameController(IGameService gameService)
        {
            this.gameService = gameService;
        }
        [HttpGet("GetMyQuestion")]
        public async Task<ActionResult> GetMyQuestions()
        {
            var id = HttpContext.User.Claims.Where(c=> c.Type== "UserId").FirstOrDefault();
            return Ok();
        }
        [Authorize(Policy = "admin")]
        [HttpPost("StartRound")]
        public async Task<ActionResult> StartRound(CancellationToken cancellation)
        {
           var result =  await gameService.StartRound(cancellation);
            if (result.IsError)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }
        [Authorize(Policy = "admin")]
        [HttpPost("EndRound")]
        public async Task<ActionResult> EndRound()
        {
            return Ok();
        }
        [HttpGet("ActiveUsers")]
        public async Task<ActionResult> GetActiveUsers()
        {
            return Ok();
        }
        [HttpGet("GetCategories")]
        public async Task<ActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var result = await gameService.GetQuestionsCategories(cancellationToken);
            if (result.IsError)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }
    }
}
