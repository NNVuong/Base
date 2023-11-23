using System.ComponentModel;
using DataTransferObjects.Base;

namespace DataTransferObjects.Request;

public class UserRequest : BaseRequest
{
    [DisplayName("Tên")] public string Name { get; set; } = "";

    [DisplayName("Email")] public string Email { get; set; } = "";
}