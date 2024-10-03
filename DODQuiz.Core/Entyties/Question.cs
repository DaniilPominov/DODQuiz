using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
