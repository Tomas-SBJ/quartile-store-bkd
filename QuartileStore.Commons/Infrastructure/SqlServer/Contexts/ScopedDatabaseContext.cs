namespace QuartileStore.Commons.Infrastructure.SqlServer.Contexts;

internal class ScopedDatabaseContext(QuartileDatabaseContext dbContext) : IScopedDatabaseContext
{
    public QuartileDatabaseContext Context { get; } = dbContext;
}