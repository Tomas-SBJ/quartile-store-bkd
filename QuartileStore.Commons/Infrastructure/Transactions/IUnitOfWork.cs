namespace QuartileStore.Commons.Infrastructure.Transactions;

public interface IUnitOfWork
{
    Task Commit();
}