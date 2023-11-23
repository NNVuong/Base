using System.Linq.Expressions;
using DataBase.Base;
using DataBase.Context;
using DataTransferObjects.Response;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Utilities.Constants;
using Utilities.Helper;
using Action = Utilities.Constants.Action;

namespace Repositories.Implements;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly MfrDbContext _dbContext;

    public Repository(MfrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Get Services

    public async Task<T?> GetElementAsync(string? id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        try
        {
            return await _dbContext.Set<T>().FindAsync(Guid.Parse(id));
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return null;
        }
    }

    public async Task<T?> GetElementAsync(Expression<Func<T, bool>> where,
        List<Expression<Func<T, dynamic?>>>? includes = null, bool asNoTracking = true)
    {
        try
        {
            var data = asNoTracking ? _dbContext.Set<T>().AsNoTracking() : _dbContext.Set<T>();

            includes?.ForEach(navigationPropertyPath => data = data.Include(navigationPropertyPath));

            return await data.FirstOrDefaultAsync(where);
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return null;
        }
    }

    public List<T>? GetList(Expression<Func<T, bool>>? where = null,
        List<Expression<Func<T, dynamic?>>>? includes = null, bool asNoTracking = true)
    {
        try
        {
            var data = asNoTracking ? _dbContext.Set<T>().AsNoTracking() : _dbContext.Set<T>();

            includes?.ForEach(navigationPropertyPath => data = data.Include(navigationPropertyPath));

            data = where != null ? data.Where(where) : data;

            return data.OrderByDescending(x => x.ModifiedAt).ToList();
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return null;
        }
    }

    public async Task<List<T>?> GetListAsync(Expression<Func<T, bool>>? where = null,
        List<Expression<Func<T, dynamic?>>>? includes = null, bool asNoTracking = true)
    {
        try
        {
            var data = asNoTracking ? _dbContext.Set<T>().AsNoTracking() : _dbContext.Set<T>();

            includes?.ForEach(navigationPropertyPath => data = data.Include(navigationPropertyPath));

            data = where != null ? data.Where(where) : data;

            return await data.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return null;
        }
    }

    public IEnumerable<T>? GetEnumerable(Expression<Func<T, bool>>? where = null,
        List<Expression<Func<T, dynamic?>>>? includes = null, bool asNoTracking = true)
    {
        try
        {
            var data = asNoTracking ? _dbContext.Set<T>().AsNoTracking() : _dbContext.Set<T>();

            includes?.ForEach(navigationPropertyPath => data = data.Include(navigationPropertyPath));

            data = where != null ? data.Where(where) : data;

            return data.OrderByDescending(x => x.ModifiedAt);
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return null;
        }
    }

    public async Task<IEnumerable<T>?> GetEnumerableAsync(Expression<Func<T, bool>>? where = null,
        List<Expression<Func<T, dynamic?>>>? includes = null, bool asNoTracking = true)
    {
        try
        {
            var data = asNoTracking ? _dbContext.Set<T>().AsNoTracking() : _dbContext.Set<T>();

            includes?.ForEach(navigationPropertyPath => data = data.Include(navigationPropertyPath));

            data = where != null ? data.Where(where) : data;

            return await data.OrderByDescending(x => x.ModifiedAt).ToListAsync();
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return null;
        }
    }

    public IQueryable<T>? GetQueryable(Expression<Func<T, bool>>? where = null,
        List<Expression<Func<T, dynamic?>>>? includes = null, bool asNoTracking = true)
    {
        try
        {
            var data = asNoTracking ? _dbContext.Set<T>().AsNoTracking() : _dbContext.Set<T>();

            includes?.ForEach(navigationPropertyPath => data = data.Include(navigationPropertyPath));

            data = where != null ? data.Where(where) : data;

            return data.OrderByDescending(x => x.ModifiedAt);
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return null;
        }
    }

    #endregion

    #region Action Services

    public async Task<MessageResponse> ActionAsync(string action, T entity, Guid userId)
    {
        var data = SetupData(action, entity, userId);

        return action switch
        {
            Action.AddAsync => await AddAsync(data),
            Action.UpdateAsync => await UpdateAsync(data),
            Action.DeleteAsync => await UpdateAsync(data),
            Action.HardDeleteAsync => await HardDeleteAsync(data),
            _ => new MessageResponse()
        };
    }

    public async Task<MessageResponse> ActionAsync(string action, IEnumerable<T> entities, Guid userId)
    {
        var data = SetupData(action, entities, userId);

        return action switch
        {
            Action.AddAsync => await AddAsync(data),
            Action.UpdateAsync => await UpdateAsync(data),
            Action.DeleteAsync => await UpdateAsync(data),
            Action.HardDeleteAsync => await HardDeleteAsync(data),
            _ => new MessageResponse()
        };
    }

    #endregion

    #region Private Functions

    private async Task<MessageResponse> AddAsync(T entity)
    {
        MessageResponse result;

        try
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            result = new MessageResponse(true, Message.Success);
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return new MessageResponse(false, Message.Failure);
        }

        return result;
    }

    private async Task<MessageResponse> AddAsync(IEnumerable<T> entities)
    {
        MessageResponse result;

        try
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
            result = new MessageResponse(true, Message.Success);
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return new MessageResponse(false, Message.Failure);
        }

        return result;
    }

    private async Task<MessageResponse> UpdateAsync(T entity)
    {
        MessageResponse result;

        try
        {
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
            result = new MessageResponse(true, Message.Success);
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return new MessageResponse(false, Message.Failure);
        }

        return result;
    }

    private async Task<MessageResponse> UpdateAsync(IEnumerable<T> entities)
    {
        MessageResponse result;

        try
        {
            _dbContext.Set<T>().UpdateRange(entities);
            await _dbContext.SaveChangesAsync();
            result = new MessageResponse(true, Message.Success);
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return new MessageResponse(false, Message.Failure);
        }

        return result;
    }

    private async Task<MessageResponse> HardDeleteAsync(T entity)
    {
        MessageResponse result;

        try
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            result = new MessageResponse(true, Message.Success);
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return new MessageResponse(false, Message.Failure);
        }

        return result;
    }

    private async Task<MessageResponse> HardDeleteAsync(IEnumerable<T> entities)
    {
        MessageResponse result;

        try
        {
            _dbContext.Set<T>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
            result = new MessageResponse(true, Message.Success);
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return new MessageResponse(false, Message.Failure);
        }

        return result;
    }

    private static T SetupData(string action, T entity, Guid userId)
    {
        var now = DateTime.Now;

        entity.ModifiedBy = userId;
        entity.ModifiedAt = now;

        switch (action)
        {
            case Action.AddAsync:
                entity.CreatedBy = userId;
                entity.CreatedAt = now;
                entity.IsDeleted = false;
                break;
            case Action.DeleteAsync:
                entity.IsDeleted = true;
                break;
        }

        return entity;
    }

    private static IEnumerable<T> SetupData(string action, IEnumerable<T> entities, Guid userId)
    {
        return entities.Select(entity => SetupData(action, entity, userId));
    }

    #endregion
}