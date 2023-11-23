using System.Linq.Expressions;
using DataBase.Base;
using DataTransferObjects.Base;
using DataTransferObjects.Response;

namespace Services.Interfaces;

public interface ICoreServices<TEntity, TRequest, TResponse>
    where TEntity : BaseEntity
    where TRequest : BaseRequest
    where TResponse : BaseResponse
{
    /// <summary>
    ///     Lấy một yêu cầu bất đồng bộ theo ID.
    /// </summary>
    Task<TRequest?> GetRequestAsync(string? id);

    /// <summary>
    ///     Lấy một phần tử bất đồng bộ theo ID.
    /// </summary>
    Task<TResponse?> GetElementAsync(string? id);

    /// <summary>
    ///     Lấy một phần tử bất đồng bộ theo điều kiện và bao gồm các thuộc tính liên quan.
    /// </summary>
    Task<TResponse?> GetElementAsync(Expression<Func<TEntity, bool>> where,
        List<Expression<Func<TEntity, dynamic?>>>? includes = null,
        bool asNoTracking = true);

    /// <summary>
    ///     Lấy danh sách các phần tử dựa trên điều kiện và bao gồm các thuộc tính liên quan.
    /// </summary>
    List<TResponse>? GetList(Expression<Func<TEntity, bool>>? where = null,
        List<Expression<Func<TEntity, dynamic?>>>? includes = null, bool asNoTracking = true);

    /// <summary>
    ///     Lấy danh sách các phần tử bất đồng bộ dựa trên điều kiện và bao gồm các thuộc tính liên quan.
    /// </summary>
    Task<List<TResponse>?> GetListAsync(Expression<Func<TEntity, bool>>? where = null,
        List<Expression<Func<TEntity, dynamic?>>>? includes = null, bool asNoTracking = true);

    /// <summary>
    ///     Tạo đối tượng PaginatedResponse từ danh sách dữ liệu và thông tin phân trang.
    /// </summary>
    PaginatedResponse<TResponse> GetPaginated(List<TResponse> data, PaginationInfo? pagination = null);

// Dịch vụ Hành động

/// <summary>
///     Thực hiện một hành động bất đồng bộ trên yêu cầu và trả về phản hồi.
/// </summary>
Task<MessageResponse> ActionAsync(string action, TRequest entity, string userId);

/// <summary>
///     Thực hiện một hành động bất đồng bộ trên tập hợp các yêu cầu và trả về phản hồi.
/// </summary>
Task<MessageResponse> ActionAsync(string action, IEnumerable<TRequest> entities, string userId);
}