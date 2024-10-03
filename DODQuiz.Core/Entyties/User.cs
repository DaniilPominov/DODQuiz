using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
