using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;

namespace Utilities.Helper;

/// <summary>
///     Lớp cung cấp phương thức tĩnh để thực hiện mã hóa và giải mã dữ liệu sử dụng GZip và Base64.
/// </summary>
public class Encryption
{
    /// <summary>
    ///     Mã hóa dữ liệu động thành chuỗi Base64 đã được nén bằng GZip.
    /// </summary>
    /// <param name="data">Dữ liệu cần mã hóa.</param>
    /// <returns>Chuỗi Base64 đã được nén.</returns>
    public static string? GetEncrypted(dynamic data)
    {
        try
        {
            var jsonString = JsonConvert.SerializeObject(data);

            var dataToCompress = Encoding.UTF8.GetBytes(jsonString);

            var compressedData = GZipCompressor.CompressAsync(dataToCompress).Result;

            var result = Convert.ToBase64String(compressedData);

            return result;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    ///     Giải mã chuỗi Base64 đã được nén bằng GZip thành dữ liệu động.
    /// </summary>
    /// <param name="data">Chuỗi Base64 đã được nén cần giải mã.</param>
    /// <returns>Dữ liệu động đã được giải mã.</returns>
    public static dynamic? GetDecoded(string? data)
    {
        if (string.IsNullOrEmpty(data) || string.IsNullOrWhiteSpace(data)) return null;

        var dataToCompress = Convert.FromBase64String(data);

        var decompressedData = GZipCompressor.DecompressAsync(dataToCompress).Result;

        var result = Encoding.UTF8.GetString(decompressedData);

        return JsonConvert.DeserializeObject<dynamic>(result);
    }
}

/// <summary>
///     Lớp tiện ích cung cấp phương thức tĩnh để thực hiện nén và giải nén dữ liệu bằng GZip.
/// </summary>
public static class GZipCompressor
{
    /// <summary>
    ///     Nén mảng byte sử dụng GZip.
    /// </summary>
    /// <param name="bytes">Mảng byte cần nén.</param>
    /// <returns>Mảng byte đã được nén.</returns>
    public static async Task<byte[]> CompressAsync(byte[] bytes)
    {
        using var memoryStream = new MemoryStream();
        await using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
        {
            await gzipStream.WriteAsync(bytes);
        }

        return memoryStream.ToArray();
    }

    /// <summary>
    ///     Giải nén mảng byte đã được nén bằng GZip.
    /// </summary>
    /// <param name="bytes">Mảng byte cần giải nén.</param>
    /// <returns>Mảng byte đã được giải nén.</returns>
    public static async Task<byte[]> DecompressAsync(byte[] bytes)
    {
        using var memoryStream = new MemoryStream(bytes);
        using var outputStream = new MemoryStream();
        await using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
        {
            await decompressStream.CopyToAsync(outputStream);
        }

        return outputStream.ToArray();
    }
}