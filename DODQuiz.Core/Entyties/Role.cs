using CSharpFunctionalExtensions;

namespace DODQuiz.Core.Entyties
{
    public class Role : Entity<Guid>
    {
        private Role() { }
        public string Name { get; set; }

        public List<User> Users { get; set; }
    }
}
