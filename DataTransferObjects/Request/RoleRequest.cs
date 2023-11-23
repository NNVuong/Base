using System.ComponentModel;
using DataTransferObjects.Base;

namespace DataTransferObjects.Request;

public class RoleRequest : BaseRequest
{
    [DisplayName("Tên quyền")] public string Name { get; set; } = "";

    [DisplayName("Mô tả")] public string Description { get; set; } = "";
}