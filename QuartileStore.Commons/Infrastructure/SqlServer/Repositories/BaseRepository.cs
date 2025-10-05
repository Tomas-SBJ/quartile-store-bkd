using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using QuartileStore.Commons.Domain.Entities.Abstracts;
using QuartileStore.Commons.Infrastructure.SqlServer.Contexts;

namespace QuartileStore.Commons.Infrastructure.SqlServer.Repositories;

internal class BaseRepository<TEntity>(
    QuartileDatabaseContext dbContext
) : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly DbSet<TEntity> Entity = dbContext.Set<TEntity>();

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        var newEntity = await Entity.AddAsync(entity);
        return newEntity.Entity;
    }
    
    public async Task<TEntity?> SelectOneByAsync(Expression<Func<TEntity, bool>> predicate) =>
        await Entity.FirstOrDefaultAsync(predicate);

    public async Task<bool> Exists(Expression<Func<TEntity, bool>> predicate) => await Entity.AnyAsync(predicate);
    
    public void Delete(TEntity entity) => Entity.Remove(entity);
}