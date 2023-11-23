using System.ComponentModel;

namespace DataTransferObjects.Response;

public class ErrorResponse
{
    [DisplayName("Request Id")] public string? RequestId { get; set; }

    [DisplayName("Nội dung")] public string? Message { get; set; }

    [DisplayName("Hiển thị Request Id?")] public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}