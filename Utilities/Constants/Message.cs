namespace Utilities.Constants;

public static class Message
{
    public const string Success = "Thao tác thành công";

    public const string Warning = "Thông tin không chính xác";

    public const string Failure = "Đã có lỗi xảy ra";

    public const string UserEmpty = "Không tìm thấy tài khoản người dùng hiện tại";

    public const string UserRoleEmpty = "Tài khoản chưa được cấp quyền";

    public const string UserCampusEmpty = "Tài khoản chưa xác định Campus";

    public const string DataNotValid = "Dữ liệu không hợp lệ";

    public const string DataNull = "Dữ liệu không tìm thấy";

    public static class Status
    {
        public static string SuccessStatus { get; } = "Thành công";
        public static string WarningStatus { get; } = "Cảnh báo";
        public static string FailureStatus { get; } = "Lỗi";
    }
}