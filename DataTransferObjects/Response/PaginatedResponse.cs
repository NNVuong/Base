using DataTransferObjects.Base;

namespace DataTransferObjects.Response;

public class PaginatedResponse<T> where T : class
{
    public List<T> Data { get; set; } = new();
    public PaginationInfo PaginationInfo { get; set; } = new();
}