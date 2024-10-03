using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DODQuiz.Application.Abstract.Security
{
    public interface IPasswordHasher
    {
        string Generate(string password);

        bool VerifyPassword(string password, string HashedPassword);
    }
}
