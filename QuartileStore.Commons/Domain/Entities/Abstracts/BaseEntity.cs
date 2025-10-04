namespace QuartileStore.Commons.Domain.Entities.Abstracts;

public abstract class BaseEntity
{
    public Guid Id { get; } = Guid.NewGuid();
}