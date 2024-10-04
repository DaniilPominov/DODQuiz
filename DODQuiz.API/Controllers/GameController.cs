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
        [HttpPost("StartRound")]
        public async Task<ActionResult> StartRound()
        {
            return Ok();
        }
        [HttpGet("ActiveUsers")]
        public async Task<ActionResult> GetActiveUsers()
        {
            return Ok();
        }
    }
}
