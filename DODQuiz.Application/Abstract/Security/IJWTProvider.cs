using DODQuiz.Core.Entyties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DODQuiz.Application.Abstract.Security
{
    public interface IJWTProvider
    {
        string GenerateToken(User user);
    }
}
