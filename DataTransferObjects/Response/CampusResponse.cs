using System.ComponentModel;
using DataTransferObjects.Base;

namespace DataTransferObjects.Response;

public class CampusResponse : BaseResponse
{
    [DisplayName("Tên campus")] public string Name { get; set; } = "";

    [DisplayName("Mã campus")] public string Code { get; set; } = "";
}