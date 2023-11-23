using System.ComponentModel;
using DataTransferObjects.Base;

namespace DataTransferObjects.Response;

public class UserCampusResponse : BaseResponse
{
    [DisplayName("Người dùng")] public string UserEmail { get; set; } = "";

    [DisplayName("Tên campus")] public string CampusName { get; set; } = "";

    [DisplayName("Mã campus")] public string CampusCode { get; set; } = "";

    public Guid UserId { get; set; } = Guid.Empty;

    public Guid CampusId { get; set; } = Guid.Empty;
}