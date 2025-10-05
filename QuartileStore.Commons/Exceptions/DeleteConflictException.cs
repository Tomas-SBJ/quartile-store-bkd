using QuartileStore.Commons.Domain.Constants;

namespace QuartileStore.Commons.Exceptions;

public class DeleteConflictException(string message) : Exception(message)
{
    public string Title { get; } = ApiErrorTitle.DeleteConflict;
}