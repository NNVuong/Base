using System.ComponentModel;
using DataTransferObjects.Base;

namespace DataTransferObjects.Request;

public class UserCampusRequest : BaseRequest
{
    [DisplayName("Người dùng")] public Guid UserId { get; set; } = Guid.Empty;

    [DisplayName("Campus")] public Guid CampusId { get; set; } = Guid.Empty;
}