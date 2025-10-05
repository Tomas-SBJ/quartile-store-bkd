using QuartileStore.Commons.Domain.Constants;

namespace QuartileStore.Commons.Exceptions;

public class EntityNotFoundException(
    string message
) : Exception(message)
{
    public string Title { get; } = ApiErrorTitle.EntityNotFound;
}