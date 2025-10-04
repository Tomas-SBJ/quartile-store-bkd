namespace QuartileStore.Commons.Infrastructure.SqlServer.Contexts;

public interface IScopedDatabaseContext
{
    QuartileDatabaseContext Context { get; }
}