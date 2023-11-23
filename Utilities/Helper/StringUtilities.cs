using System.Text.RegularExpressions;

namespace Utilities.Helper;

/// <summary>
///     Cung cấp các tiện ích xử lý chuỗi liên quan đến Unicode và tạo URL thân thiện.
/// </summary>
public static class StringUtilities
{
    /// <summary>
    ///     Chuyển đổi chuỗi thành Unicode và tạo URL thân thiện.
    /// </summary>
    /// <param name="input">Chuỗi cần xử lý.</param>
    /// <returns>Chuỗi đã được chuyển đổi thành Unicode và tạo URL thân thiện.</returns>
    public static string ToUnicode(string input)
    {
        // Chuyển đổi chuỗi thành chữ thường
        input = input.ToLower();

        // Loại bỏ dấu tiếng Việt và các ký tự không hợp lệ
        input = Regex.Replace(input, @"[áàạảãâấầậẩẫăắằặẳẵ]", "a");
        input = Regex.Replace(input, @"[éèẹẻẽêếềệểễ]", "e");
        input = Regex.Replace(input, @"[óòọỏõôốồộổỗơớờợởỡ]", "o");
        input = Regex.Replace(input, @"[íìịỉĩ]", "i");
        input = Regex.Replace(input, @"[ýỳỵỉỹ]", "y");
        input = Regex.Replace(input, @"[úùụủũưứừựửữ]", "u");
        input = Regex.Replace(input, @"đ", "d");

        // Chỉ chấp nhận các ký tự sau: [0-9a-z-\s]
        input = Regex.Replace(input.Trim(), @"[^0-9a-z-\s]", "").Trim();

        // Xử lý nhiều hơn 1 khoảng trắng thành 1 khoảng trắng
        input = Regex.Replace(input.Trim(), @"\s+", "_");

        // Thay thế khoảng trắng bằng dấu gạch dưới
        input = Regex.Replace(input, @"\s", "_");

        // Xử lý nhiều hơn 1 dấu gạch dưới liên tiếp thành 1 dấu gạch dưới
        while (input.IndexOf("__", StringComparison.Ordinal) != -1) input = input.Replace("__", "_");

        return input;
    }
}