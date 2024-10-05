namespace DODQuiz.Contracts
{
    public record QuestionResponse
    (
        Guid id,
        string name,
        string description,
        string category,
        string? imageUri
        );
}
