using DODQuiz.Application.Abstract.Services;
using DODQuiz.Contracts;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DODQuiz.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly IProfileService profileService;
        public LoginController(IProfileService profileService) 
        {
            this.profileService = profileService;
        }
        [HttpPost]
        public async Task<ActionResult> Login(UserLoginRequest loginRequest)
        {
            var userToken = await profileService.Login(loginRequest.username, loginRequest.password);
            if (userToken.IsError == true)
            {
                return BadRequest(userToken.Errors);
            }
            HttpContext.Response.Cookies.Append("bivis-bober",  userToken.Value);
            return Ok(userToken);
        }
    }
}
