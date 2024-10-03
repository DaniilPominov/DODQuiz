using CSharpFunctionalExtensions;
using ErrorOr;

namespace DODQuiz.Core.Entyties
{
    public class Role : Entity<Guid>
    {
        private Role() { }
        public string Name { get; set; }

        public List<User> Users { get; set; }

        public static ErrorOr<Role> Create(Guid id, string name, List<User> users)
        {
            return new Role() { Id=id,Name=name,Users=users };
        }
    }
}
