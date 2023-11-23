using System.ComponentModel;
using DataTransferObjects.Base;

namespace DataTransferObjects.Response;

public class CategoryResponse : BaseResponse
{
    [DisplayName("Tên danh mục")] public string Name { get; set; } = "";
}