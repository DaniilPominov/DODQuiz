using CSharpFunctionalExtensions;

namespace DODQuiz.Core.Entyties
{
    public class User : Entity<Guid>
    {
        private User() { }

        public string Name { get; set; }

        public string HashPass { get; set; }

        public List<Role> Roles { get; set; }

    }
}
