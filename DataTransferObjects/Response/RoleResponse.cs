using System.ComponentModel;
using DataTransferObjects.Base;

namespace DataTransferObjects.Response;

public class RoleResponse : BaseResponse
{
    [DisplayName("Tên quyền")] public string Name { get; set; } = "";

    [DisplayName("Mô tả")] public string Description { get; set; } = "";
}