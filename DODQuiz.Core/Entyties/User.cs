using CSharpFunctionalExtensions;
using ErrorOr;

namespace DODQuiz.Core.Entyties
{
    public class User : Entity<Guid>
    {
        private User() { }

        public string Name { get; set; }

        public string HashPass { get; set; }

        public List<Role> Roles { get; set; }
        public static ErrorOr<User> Create(Guid id, string name, List<Role> roles, string password)
        {
            return new User() { Id = id, Name = name, Roles = roles, HashPass = password };
        }

    }
}
