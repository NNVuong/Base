namespace Utilities.Helper;

/// <summary>
///     Lớp tiện ích cho việc ghi log phát triển.
/// </summary>
public static class Developer
{
    /// <summary>
    ///     Ghi log dữ liệu động vào console.
    /// </summary>
    /// <param name="data">Dữ liệu động cần ghi log.</param>
    public static void WriteLog(dynamic? data)
    {
        Console.WriteLine($"LOG:\n{data}");
    }
}