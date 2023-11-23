using System.ComponentModel;

namespace DataTransferObjects.Base;

public class PaginationInfo
{
    private const int DefaultCurrentPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;
    private readonly string _defaultKeyword = string.Empty;

    private int _currentPage = DefaultCurrentPage;
    private string _keyword = string.Empty;
    private int _pageSize = DefaultPageSize;

    [DisplayName("Trang hiện tại")]
    public int CurrentPage
    {
        get => _currentPage;
        set => _currentPage = value > DefaultCurrentPage ? value : DefaultCurrentPage;
    }

    [DisplayName("Số phần tử mỗi trang")]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > DefaultPageSize ? Math.Min(value, MaxPageSize) : DefaultPageSize;
    }

    [DisplayName("Tìm kiếm")]
    public string? Keyword
    {
        get => _keyword;
        set => _keyword = string.IsNullOrEmpty(value) ? _defaultKeyword : value;
    }

    [DisplayName("Tổng số phần tử")] public int TotalItems { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}