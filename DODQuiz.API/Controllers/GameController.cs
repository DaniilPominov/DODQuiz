using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DODQuiz.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy ="user")]
    [ApiController]
    public class GameController : ControllerBase
    {
        [HttpGet("Question")]
        public async Task<ActionResult> GetQuestion(string username)
        {
            return Ok();
        }
    }
}
