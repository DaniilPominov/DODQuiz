using DODQuiz.Application.Abstract.Repos;
using DODQuiz.Core.Entyties;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DODQuiz.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        IRoleRepos roleRepos;
        public DebugController(IRoleRepos roleRepos)
        {
            this.roleRepos = roleRepos;
        }
        [HttpPost("CreateRole")]

        public async Task<ActionResult> CreateRole(string name,CancellationToken cancellationToken)
        {
            var role = Role.Create(Guid.NewGuid(), name, new List<User>()).Value;
            await roleRepos.AddAsync(role, cancellationToken);
            return Ok();
        }
    }
}
