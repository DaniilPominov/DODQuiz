using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DODQuiz.Contracts
{
    public record UserResponse
    (
        Guid id,
        string name

        );
}
