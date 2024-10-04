using DODQuiz.Application.Abstract.Services;
using DODQuiz.Contracts;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
            HttpContext.Response.Cookies.Append("bivis-bober", userToken.Value);
            return Ok(userToken);
        }
        [HttpGet("CheckRoles")]
        public async Task<ActionResult> CheckRoles()
        {
            var userRoles = HttpContext.User.Claims.Where(c=> c.Type == ClaimTypes.Role).ToList();
            if (userRoles.Count == 0)
            {
                return BadRequest();
                
            }
            var Roles = new List<string>();
            foreach (var role in userRoles)
            {
                Roles.Add(role.Value);
            }

            return Ok(new RolesResponse(userroles:Roles));
        }
    }
}
