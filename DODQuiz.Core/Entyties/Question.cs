using CSharpFunctionalExtensions;

namespace DODQuiz.Core.Entyties
{
    public class Question : Entity<Guid>
    {
        private Question() { }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ImageUri { get; set; } = null!;
    }
}
