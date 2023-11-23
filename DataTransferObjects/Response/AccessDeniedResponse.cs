using System.ComponentModel;

namespace DataTransferObjects.Response;

public class AccessDeniedResponse
{
    [DisplayName("Nội dung")] public string? Message { get; set; }
}