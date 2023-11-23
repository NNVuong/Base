using System.Linq.Expressions;
using AutoMapper;
using DataBase.Base;
using DataTransferObjects.Base;
using DataTransferObjects.Response;
using Repositories.Interfaces;
using Services.Interfaces;
using Utilities.Constants;
using Utilities.Helper;

namespace Services.Implements;

public class CoreServices<TEntity, TRequest, TResponse> : ICoreServices<TEntity, TRequest, TResponse>
    where TEntity : BaseEntity
    where TRequest : BaseRequest
    where TResponse : BaseResponse
{
    private readonly IMapper _mapper;
    private readonly IRepository<TEntity> _repository;

    public CoreServices(IMapper mapper, IRepository<TEntity> repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    #region Get Services

    public async Task<TRequest?> GetRequestAsync(string? id)
    {
        var data = await _repository.GetElementAsync(id);
        return _mapper.Map<TEntity?, TRequest?>(data);
    }

    public async Task<TResponse?> GetElementAsync(string? id)
    {
        var data = await _repository.GetElementAsync(id);
        return _mapper.Map<TEntity?, TResponse?>(data);
    }

    public async Task<TResponse?> GetElementAsync(Expression<Func<TEntity, bool>> where,
        List<Expression<Func<TEntity, dynamic?>>>? includes = null, bool asNoTracking = true)
    {
        var data = await _repository.GetElementAsync(where, includes, asNoTracking);
        return _mapper.Map<TEntity?, TResponse?>(data);
    }

    public List<TResponse>? GetList(Expression<Func<TEntity, bool>>? where = null,
        List<Expression<Func<TEntity, dynamic?>>>? includes = null, bool asNoTracking = true)
    {
        var data = _repository.GetList(where, includes, asNoTracking);
        return _mapper.Map<List<TEntity>?, List<TResponse>?>(data);
    }

    public async Task<List<TResponse>?> GetListAsync(Expression<Func<TEntity, bool>>? where = null,
        List<Expression<Func<TEntity, dynamic?>>>? includes = null, bool asNoTracking = true)
    {
        var data = await _repository.GetListAsync(where, includes, asNoTracking);
        return _mapper.Map<List<TEntity>?, List<TResponse>?>(data);
    }

    public PaginatedResponse<TResponse> GetPaginated(List<TResponse> data, PaginationInfo? pagination = null)
    {
        pagination ??= new PaginationInfo();

        if (!string.IsNullOrEmpty(pagination.Keyword))
            data = data
                .Where(x => x.GetType().GetProperties()
                    .Any(p => $"{p.GetValue(x)}".ToUpper().Contains(pagination.Keyword.ToUpper())))
                .ToList();

        var resultData = data
            .Skip((pagination.CurrentPage - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();

        var paginatedResult = new PaginatedResponse<TResponse>
        {
            Data = resultData,
            PaginationInfo = new PaginationInfo
            {
                Keyword = pagination.Keyword,
                CurrentPage = pagination.CurrentPage,
                PageSize = pagination.PageSize,
                TotalItems = data.Count
            }
        };

        return paginatedResult;
    }

    #endregion

    #region Action Services

    public async Task<MessageResponse> ActionAsync(string action, TRequest data, string userId)
    {
        try
        {
            var entity = await SetupData(data);

            return await _repository.ActionAsync(action, entity, Guid.Parse(userId));
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return new MessageResponse(false, Message.Failure);
        }
    }

    public async Task<MessageResponse> ActionAsync(string action, IEnumerable<TRequest> data, string userId)
    {
        try
        {
            var entities = await SetupData(data);

            return await _repository.ActionAsync(action, entities, Guid.Parse(userId));
        }
        catch (Exception ex)
        {
            Developer.WriteLog(ex.Message);
            return new MessageResponse(false, Message.Failure);
        }
    }

    #endregion

    #region Private

    private async Task<TEntity> SetupData(TRequest data)
    {
        var baseEntity = await _repository.GetElementAsync(data.Id.ToString());

        if (baseEntity != null) baseEntity = _mapper.Map(data, baseEntity);

        var entity = baseEntity ?? _mapper.Map<TRequest, TEntity>(data);

        return entity;
    }

    private async Task<List<TEntity>> SetupData(IEnumerable<TRequest> listData)
    {
        var entities = new List<TEntity>();

        foreach (var data in listData)
        {
            var entity = await SetupData(data);
            entities.Add(entity);
        }

        return entities;
    }

    #endregion
}