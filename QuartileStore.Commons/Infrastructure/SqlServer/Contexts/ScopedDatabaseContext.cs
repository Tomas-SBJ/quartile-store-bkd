namespace QuartileStore.Commons.Infrastructure.SqlServer.Contexts;

public class ScopedDatabaseContext(QuartileDatabaseContext context) : IScopedDatabaseContext
{
    public QuartileDatabaseContext Context { get; } = context;
}