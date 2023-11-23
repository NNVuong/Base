using System.Linq.Expressions;
using DataTransferObjects.Response;

namespace Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    /// <summary>
    ///     Lấy một phần tử bất đồng bộ theo ID.
    /// </summary>
    Task<T?> GetElementAsync(string? id);

    /// <summary>
    ///     Lấy một phần tử bất đồng bộ theo điều kiện và bao gồm các thuộc tính liên quan.
    /// </summary>
    Task<T?> GetElementAsync(Expression<Func<T, bool>> where, List<Expression<Func<T, dynamic?>>>? includes = null,
        bool asNoTracking = true);

    /// <summary>
    ///     Lấy danh sách các phần tử dựa trên điều kiện và bao gồm các thuộc tính liên quan.
    /// </summary>
    List<T>? GetList(Expression<Func<T, bool>>? where = null,
        List<Expression<Func<T, dynamic?>>>? includes = null, bool asNoTracking = true);

    /// <summary>
    ///     Lấy danh sách các phần tử bất đồng bộ dựa trên điều kiện và bao gồm các thuộc tính liên quan.
    /// </summary>
    Task<List<T>?> GetListAsync(Expression<Func<T, bool>>? where = null,
        List<Expression<Func<T, dynamic?>>>? includes = null, bool asNoTracking = true);

    /// <summary>
    ///     Lấy IEnumerable các phần tử dựa trên điều kiện và bao gồm các thuộc tính liên quan.
    /// </summary>
    IEnumerable<T>? GetEnumerable(Expression<Func<T, bool>>? where = null,
        List<Expression<Func<T, dynamic?>>>? includes = null, bool asNoTracking = true);

    /// <summary>
    ///     Lấy IEnumerable các phần tử bất đồng bộ dựa trên điều kiện và bao gồm các thuộc tính liên quan.
    /// </summary>
    Task<IEnumerable<T>?> GetEnumerableAsync(Expression<Func<T, bool>>? where = null,
        List<Expression<Func<T, dynamic?>>>? includes = null, bool asNoTracking = true);

    /// <summary>
    ///     Lấy IQueryable các phần tử dựa trên điều kiện và bao gồm các thuộc tính liên quan.
    /// </summary>
    IQueryable<T>? GetQueryable(Expression<Func<T, bool>>? where = null,
        List<Expression<Func<T, dynamic?>>>? includes = null, bool asNoTracking = true);

    /// <summary>
    ///     Thực hiện một hành động bất đồng bộ trên phần tử và trả về phản hồi.
    /// </summary>
    Task<MessageResponse> ActionAsync(string action, T entity, Guid userId);

    /// <summary>
    ///     Thực hiện một hành động bất đồng bộ trên tập hợp các phần tử và trả về phản hồi.
    /// </summary>
    Task<MessageResponse> ActionAsync(string action, IEnumerable<T> entities, Guid userId);
}