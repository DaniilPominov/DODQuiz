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
