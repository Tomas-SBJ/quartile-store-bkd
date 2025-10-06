using QuartileStore.Commons.Domain.Constants;

namespace QuartileStore.Commons.Application.Exceptions;

public class EntityAlreadyExistsException(string message) : Exception(message)
{
    public string Title { get; } = ApiErrorTitle.EntityAlreadyExists;
}