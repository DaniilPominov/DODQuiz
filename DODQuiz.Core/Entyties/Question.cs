using CSharpFunctionalExtensions;
using ErrorOr;

namespace DODQuiz.Core.Entyties
{
    public class Question : Entity<Guid>
    {
        private Question() { }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ImageUri { get; set; } = null!;
        public static ErrorOr<Question> Create(Guid id,string name, string Description, string Category, string ImageUri)
        {
            return new Question() {Id = id, Name = name, Description = Description, Category = Category, ImageUri = ImageUri };
        }
    }
}
