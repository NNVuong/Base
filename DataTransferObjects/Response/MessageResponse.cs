using System.ComponentModel;

namespace DataTransferObjects.Response;

public class MessageResponse
{
    public MessageResponse()
    {
        IsSuccess = null;
        Message = Utilities.Constants.Message.Warning;
        //Data = null;
    }

    public MessageResponse(bool? isSuccess, string? message //, dynamic? data
    )
    {
        IsSuccess = isSuccess;
        Message = string.IsNullOrEmpty(message)
            ? isSuccess == null
                ? Utilities.Constants.Message.Warning
                : isSuccess == true
                    ? Utilities.Constants.Message.Success
                    : Utilities.Constants.Message.Failure
            : message;
        //Data = data;
    }

    public MessageResponse(string message)
    {
        IsSuccess = null;
        Message = message;
        //Data = data;
    }

    public MessageResponse(MessageResponse messageResponse)
    {
        IsSuccess = messageResponse.IsSuccess;
        Message = messageResponse.Message;
        //Data = messageResponse.Data;
    }

    [DisplayName("Thành công?")] public bool? IsSuccess { get; set; }

    [DisplayName("Nội dung")] public string Message { get; set; }

    //[DisplayName("Dữ liệu")]
    public dynamic? Data { get; set; }
}