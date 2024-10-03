using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DODQuiz.Core.Entyties
{
    public class Role : Entity<Guid>
    {
        private Role() { }
        public string Name { get; set; }

        public List<User> Users { get; set; }
    }
}
