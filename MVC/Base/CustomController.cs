using System.Security.Claims;
using DataTransferObjects.Base;
using DataTransferObjects.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using Utilities.Constants;

namespace MVC.Base;

public class CustomController : Controller
{
    private readonly string _area;
    private readonly string _title;

    protected CustomController(string title, string area)
    {
        _title = title;
        _area = area;
    }

    /// <summary>
    ///     Phương thức không thực hiện, chuyển hướng đến trang chủ.
    /// </summary>
    [NonAction]
    protected RedirectToActionResult RedirectToHome()
    {
        return RedirectToAction(DefaultValue.Action, DefaultValue.Controller,
            new { area = DefaultValue.Area });
    }

    /// <summary>
    ///     Phương thức không thực hiện, chuyển hướng đến trang chính.
    /// </summary>
    [NonAction]
    protected RedirectToActionResult RedirectToIndex()
    {
        return RedirectToAction("Index", null, new { area = _area });
    }

    /// <summary>
    ///     Phương thức không thực hiện, chuyển hướng đến trang thêm mới.
    /// </summary>
    [NonAction]
    protected RedirectToActionResult RedirectToCreate()
    {
        return RedirectToAction("Create", null, new { area = _area });
    }

    /// <summary>
    ///     Phương thức không thực hiện, chuyển hướng đến trang chỉnh sửa.
    /// </summary>
    [NonAction]
    protected RedirectToActionResult RedirectToEdit()
    {
        return RedirectToAction("Edit", null, new { area = _area });
    }

    #region Override View()

    /// <summary>
    ///     Tạo một đối tượng <see cref="ViewResult" /> bằng cách chỉ định một <paramref name="viewName" />
    ///     và <paramref name="model" /> sẽ được hiển thị bởi view.
    /// </summary>
    /// <param name="viewName">Tên hoặc đường dẫn của view sẽ được hiển thị trong phản hồi.</param>
    /// <param name="model">Dữ liệu mà view sẽ hiển thị.</param>
    /// <returns>Đối tượng <see cref="ViewResult" /> được tạo cho phản hồi.</returns>
    [NonAction]
    public override ViewResult View(string? viewName, object? model)
    {
        ViewBag.UserInfomation = new
        {
            UserId = GetCurrentUserId(),
            UserName = GetCurrentUserName(),
            UserEmail = GetCurrentUserEmail(),
            UserRole = GetCurrentUserRole(),
            UserCampusCode = GetCurrentUserCampusCode(),
            UserCampusName = GetCurrentUserCampusName()
        };

        ViewBag.Title = _title;

        var message = GetMessage();

        if (message != null) ViewBag.MessageResponse = message;

        return base.View(viewName, model);
    }

    #endregion

    #region Custom Functions

    /// <summary>
    ///     Lấy thông báo từ sessionStorage.
    /// </summary>
    private MessageResponse? GetMessage(bool deleteAfterGet = true)
    {
        return GetSessionStorage<MessageResponse>(SessionType.Message, deleteAfterGet);
    }

    /// <summary>
    ///     Lấy thông tin phân trang từ sessionStorage.
    /// </summary>
    private protected PaginationInfo GetPaginationInfo(bool deleteAfterGet = true)
    {
        return GetSessionStorage<PaginationInfo>(SessionType.PaginationInfo, deleteAfterGet) ?? new PaginationInfo();
    }

    /// <summary>
    ///     Lấy giá trị từ sessionStorage.
    /// </summary>
    private protected T? GetSessionStorage<T>(string key, bool deleteAfterGet = true) where T : class
    {
        var sessionStorage = HttpContext.Session.GetString(key);

        if (deleteAfterGet) SetSessionStorage(key, string.Empty);

        if (string.IsNullOrEmpty(sessionStorage)) return null;

        try
        {
            return JsonConvert.DeserializeObject<T>(sessionStorage);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    ///     Thiết lập thông báo trong sessionStorage.
    /// </summary>
    private protected void SetMessage(string? message)
    {
        SetMessage(new MessageResponse(null, message));
    }

    /// <summary>
    ///     Thiết lập thông báo trong sessionStorage.
    /// </summary>
    private protected void SetMessage(MessageResponse messageResponse)
    {
        SetSessionStorage(SessionType.Message, messageResponse.ToJson());
    }

    /// <summary>
    ///     Thiết lập thông tin phân trang trong sessionStorage.
    /// </summary>
    private protected void SetPaginationInfo()
    {
        SetPaginationInfo(new PaginationInfo());
    }

    /// <summary>
    ///     Thiết lập thông tin phân trang trong sessionStorage.
    /// </summary>
    private protected void SetPaginationInfo(string keyword, int currentPage, int pageSize)
    {
        SetPaginationInfo(new PaginationInfo { Keyword = keyword, CurrentPage = currentPage, PageSize = pageSize });
    }

    /// <summary>
    ///     Thiết lập thông tin phân trang trong sessionStorage.
    /// </summary>
    private protected void SetPaginationInfo(PaginationInfo paginationInfo)
    {
        SetSessionStorage(SessionType.PaginationInfo, paginationInfo.ToJson());
    }

    /// <summary>
    ///     Thiết lập giá trị trong sessionStorage.
    /// </summary>
    private protected void SetSessionStorage(string key, dynamic value)
    {
        HttpContext.Session.SetString(key, $"{value}");
    }

    /// <summary>
    ///     Lấy dữ liệu Id người dùng hiện tại
    /// </summary>
    private protected string? GetCurrentUserId()
    {
        return User.Claims.FirstOrDefault(x => x.Type == UserClaimTypes.UserId)?.Value;
    }

    /// <summary>
    ///     Lấy dữ liệu Id người dùng hiện tại
    /// </summary>
    private string? GetCurrentUserName()
    {
        return User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
    }

    /// <summary>
    ///     Lấy dữ liệu Email người dùng hiện tại
    /// </summary>
    private string? GetCurrentUserEmail()
    {
        return User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
    }

    /// <summary>
    ///     Lấy dữ liệu quyền truy cập người dùng hiện tại
    /// </summary>
    private string[] GetCurrentUserRole()
    {
        return User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();
    }

    /// <summary>
    ///     Lấy dữ liệu mã campus người dùng hiện tại
    /// </summary>
    private string[] GetCurrentUserCampusCode()
    {
        return User.Claims.Where(x => x.Type == UserClaimTypes.CampusCode).Select(x => x.Value).ToArray();
    }

    /// <summary>
    ///     Lấy dữ liệu tên campus người dùng hiện tại
    /// </summary>
    private string[] GetCurrentUserCampusName()
    {
        return User.Claims.Where(x => x.Type == UserClaimTypes.CampusName).Select(x => x.Value).ToArray();
    }

    #endregion
}