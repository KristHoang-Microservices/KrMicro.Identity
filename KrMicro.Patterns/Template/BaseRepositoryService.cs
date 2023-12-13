using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace KrMicro.Patterns.Template;

public interface IBaseService<TBaseEntity> where TBaseEntity : class, new()
{
    Task<TBaseEntity> InsertAsync(TBaseEntity entity);
    Task<TBaseEntity> AttachAsync(TBaseEntity entity);
    Task<IEnumerable<TBaseEntity>> GetAllAsync(string[]? includes = null);

    Task<TBaseEntity?> GetDetailAsync(Expression<Func<TBaseEntity, bool>> predicate);

    Task<bool> CheckExistsAsync(Expression<Func<TBaseEntity, bool>> predicate);
    Task<TBaseEntity> UpdateAsync(TBaseEntity entity);
    Task<bool> DeleteAsync(TBaseEntity entity);
}

public class BaseRepositoryService<TEntity, TDbContext> : IBaseService<TEntity>
    where TEntity : class, new() where TDbContext : DbContext, new()
{
    protected readonly TDbContext DataContext;

    public BaseRepositoryService(TDbContext dataContext)
    {
        DataContext = dataContext;
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException($"{nameof(InsertAsync)} entity must not be null");

        try
        {
            await DataContext.AddAsync(entity);
            await DataContext.SaveChangesAsync();

            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}");
        }
    }

    public async Task<TEntity> AttachAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException($"{nameof(UpdateAsync)} entity must not be null");

        try
        {
            DataContext.Attach(entity);
            await DataContext.SaveChangesAsync();

            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception($"{nameof(entity)} could not be updated {ex.Message}");
        }
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException($"{nameof(UpdateAsync)} entity must not be null");

        try
        {
            DataContext.Update(entity);
            DataContext.Entry(entity).State = EntityState.Modified;
            await DataContext.SaveChangesAsync();

            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception($"{nameof(entity)} could not be updated {ex.Message}");
        }
    }

    public Task<bool> DeleteAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException($"{nameof(DeleteAsync)} entity must not be null");

        try
        {
            DataContext.Remove(entity);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            throw new Exception($"{nameof(entity)} could not be removed {ex.Message}");
        }
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(string[]? includes = null)
    {
        try
        {
            var result = DataContext.Set<TEntity>();

            if (includes is not null)
                foreach (var include in includes)
                    result.Include(include);

            return await result.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Couldn't retrieve entities: {ex.Message}");
        }
    }

    public async Task<bool> CheckExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            return await DataContext.Set<TEntity>()
                .AsNoTracking()
                .AnyAsync(predicate);
        }
        catch (Exception ex)
        {
            throw new Exception($"Couldn't retrieve entities: {ex.Message}");
        }
    }

    public async Task<TEntity?> GetDetailAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var result = DataContext.Set<TEntity>()
                .AsNoTracking().Where(predicate);

            return await result.FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Couldn't retrieve entities: {ex.Message}");
        }
    }
}