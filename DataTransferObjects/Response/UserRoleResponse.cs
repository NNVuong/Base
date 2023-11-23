using System.ComponentModel;
using DataTransferObjects.Base;

namespace DataTransferObjects.Response;

public class UserRoleResponse : BaseResponse
{
    [DisplayName("Người dùng")] public string UserEmail { get; set; } = "";

    [DisplayName("Quyền truy cập")] public string RoleName { get; set; } = "";


    public Guid UserId { get; set; } = Guid.Empty;

    public Guid RoleId { get; set; } = Guid.Empty;
}