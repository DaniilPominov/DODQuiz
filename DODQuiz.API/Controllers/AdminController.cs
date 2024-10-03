using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace DODQuiz.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet("ActiveUsers")]
        public async Task<ActionResult> GetActiveUsers()
        {
            return Ok();
        }
        [HttpPost("StartRound")]
        public async Task<ActionResult> StartRound()
        {
            return Ok();
        }
        [HttpPost("EndRound")]
        public async Task<ActionResult> EndRound()
        {
            return Ok();
        }
        [HttpPost("EditRound")]
        public async Task<ActionResult> EditRound()
        {
            return Ok();
        }
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(LoginRequest loginRequest)
        {
            return Ok();
        }
        [HttpPut("EditUser")]
        public async Task<ActionResult> EditUser()
        {
            return Ok();
        }
        [HttpDelete("DeleteUser")]
        public async Task<ActionResult> DeleteUser()
        {
            return Ok();
        }
    }
}
