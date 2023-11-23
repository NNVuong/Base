using System.ComponentModel;
using DataTransferObjects.Base;

namespace DataTransferObjects.Request;

public class CategoryDetailsRequest : BaseRequest
{
    [DisplayName("Tên chi tiết danh mục")] public string Name { get; set; } = "";

    [DisplayName("Danh mục")] public Guid CategoryId { get; set; } = Guid.Empty;
}