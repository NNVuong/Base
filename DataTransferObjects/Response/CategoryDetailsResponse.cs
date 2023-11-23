using System.ComponentModel;
using DataTransferObjects.Base;

namespace DataTransferObjects.Response;

public class CategoryDetailsResponse : BaseResponse
{
    [DisplayName("Tên chi tiết danh mục")] public string Name { get; set; } = "";

    [DisplayName("Danh mục")] public string CategoryName { get; set; } = "";

    public Guid CategoryId { get; set; } = Guid.Empty;
}