using DODQuiz.Application.Abstract.Repos;
using DODQuiz.Application.Abstract.Security;
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
    public class ProfileService : IProfileService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepos _userRepository;
        private readonly IRoleRepos _roleRepository;
        private readonly IJWTProvider _jwtProvider;

        public ProfileService(IPasswordHasher passwordHasher, IUserRepos userRepository, IRoleRepos roleRepository, IJWTProvider jwtProvider)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _jwtProvider = jwtProvider;
        }
        private async Task<ErrorOr<List<Role>>> GetUserRolesAsync(CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetAllAsync(cancellationToken);

            foreach (var role in roles.Value) { await _roleRepository.Attach(role, cancellationToken); }

            return roles;
        }
        private async Task<ErrorOr<User>> CreateUserAsync(

            UserRegisterRequest userRegisterRequest,
            CancellationToken cancellationToken)
        {
            var hashedPassword = _passwordHasher.Generate(userRegisterRequest.password);

            var userRoles = await GetUserRolesAsync(cancellationToken);
            var userRole = userRoles.Value.Where(r=> r.Name=="user").FirstOrDefault();

            return User.Create(
                id:Guid.NewGuid(),
                name:userRegisterRequest.name,
                password:hashedPassword,
                roles:new List<Role>() { userRole ?? Role.Create(Guid.NewGuid(),"user",new List<User>()).Value });

        }
        public async Task<ErrorOr<string>> Login(string Name, string Password)
        {
            var user = await _userRepository.GetUserByName(Name, CancellationToken.None);

            if (user.IsError == true)
            {
                return user.Errors;
            }

            var result = _passwordHasher.VerifyPassword(Password, user.Value.HashPass);

            if (result == false)
            {
                return Error.Failure("Wrong Name or password.");
            }

            var token = _jwtProvider.GenerateToken(user.Value);

            return token;
        }

        public async Task<ErrorOr<Success>> Register(UserRegisterRequest userRegistrationRequest, CancellationToken cancellationToken)
        {
            var user = await CreateUserAsync(userRegistrationRequest, cancellationToken);

            return await _userRepository.AddAsync(user.Value, cancellationToken);
        }
    }
}
