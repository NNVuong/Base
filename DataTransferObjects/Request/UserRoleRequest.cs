using System.ComponentModel;
using DataTransferObjects.Base;

namespace DataTransferObjects.Request;

public class UserRoleRequest : BaseRequest
{
    [DisplayName("Người dùng")] public Guid UserId { get; set; } = Guid.Empty;

    [DisplayName("Quyền")] public Guid RoleId { get; set; } = Guid.Empty;
}