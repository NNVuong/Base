using System.ComponentModel;
using DataTransferObjects.Base;

namespace DataTransferObjects.Request;

public class CategoryRequest : BaseRequest
{
    [DisplayName("Tên danh mục")] public string Name { get; set; } = "";
}