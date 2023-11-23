using System.ComponentModel;
using DataTransferObjects.Base;

namespace DataTransferObjects.Response;

public class UserResponse : BaseResponse
{
    [DisplayName("Tên")] public string Name { get; set; } = "";

    [DisplayName("Email")] public string Email { get; set; } = "";
}