using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DODQuiz.Contracts
{
    public record QuestionRequest
    (
        string name,
        string description,
        string category,
        string? imageUri
        );
}
