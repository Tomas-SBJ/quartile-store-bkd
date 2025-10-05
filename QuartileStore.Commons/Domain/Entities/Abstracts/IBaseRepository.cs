using System.Linq.Expressions;

namespace QuartileStore.Commons.Domain.Entities.Abstracts;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> CreateAsync(TEntity entity);
    Task<bool> Exists(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> SelectOneByAsync(Expression<Func<TEntity, bool>> predicate);
    void Delete(TEntity entity);
}