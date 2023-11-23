using DataBase.Base;
using DataTransferObjects.Base;
using DataTransferObjects.Response;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using Services.Interfaces;
using Utilities.Constants;
using Action = Utilities.Constants.Action;

namespace MVC.Base;

public class ManagementController<TEntity, TRequest, TResponse> : CustomController
    where TEntity : BaseEntity where TRequest : BaseRequest, new() where TResponse : BaseResponse
{
    private readonly ICoreServices<TEntity, TRequest, TResponse> _mainServices;

    public ManagementController(string title, string area, ICoreServices<TEntity, TRequest, TResponse> mainServices) :
        base(title, area)
    {
        _mainServices = mainServices;
    }

    #region HttpMethods

    /// <summary>
    ///     Lấy dữ liệu tìm kiếm và phân trang để chuyển về trang chính
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Get(PaginationInfo paginationInfo)
    {
        // Đặt lại trang hiện tại về 1 và thiết lập thông tin phân trang
        paginationInfo.CurrentPage = 1;
        SetPaginationInfo(paginationInfo);
        return RedirectToIndex();
    }

    /// <summary>
    ///     Thiết lập dữ liệu tìm kiếm và phân trang để chuyển về trang chính
    /// </summary>
    public IActionResult Get(string keyword, int currentPage, int pageSize)
    {
        // Thiết lập thông tin phân trang dựa trên tham số và chuyển hướng về trang chính
        SetPaginationInfo(keyword, currentPage, pageSize);
        return RedirectToIndex();
    }

    /// <summary>
    ///     Thiết lập dữ liệu chuyển hướng về View tạo mới
    /// </summary>
    public virtual IActionResult Post()
    {
        // Đặt lại thông tin phân trang
        SetPaginationInfo();

        var data = new TRequest();

        SetModelData(data);

        return RedirectToCreate();
    }

    /// <summary>
    ///     Thiết lập dữ liệu chuyển hướng về View chỉnh sửa
    /// </summary>
    public virtual async Task<IActionResult> Put(string? id, string keyword, int currentPage, int pageSize)
    {
        // Thiết lập thông tin phân trang
        SetPaginationInfo(keyword, currentPage, pageSize);

        var data = await GetRequest(id);

        if (data == null)
        {
            SetMessage(Message.DataNull);
            return RedirectToIndex();
        }

        SetModelData(data);
        return RedirectToEdit();
    }

    /// <summary>
    ///     Hành động để xóa một bản ghi
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Delete(string? id, string keyword, int currentPage, int pageSize)
    {
        // Thiết lập thông tin phân trang
        SetPaginationInfo(keyword, currentPage, pageSize);

        var data = await GetRequest(id);

        return await ProcessActionAsync(Action.DeleteAsync, data);
    }

    #endregion

    #region Process View

    /// <summary>
    ///     Giao diện chính
    /// </summary>
    public virtual async Task<IActionResult> Index()
    {
        // Lấy danh sách các response từ dịch vụ
        var listResponse = await GetListResponse();

        // Kiểm tra nếu danh sách là null
        if (listResponse == null)
        {
            // Thiết lập thông báo và chuyển hướng về trang chủ
            SetMessage(Message.DataNull);
            return RedirectToHome();
        }

        // Lấy dữ liệu phân trang dựa trên danh sách
        var paginatedResponse = GetPaginatedResponse(listResponse);

        // Trả về view với dữ liệu phân trang
        ViewBag.PaginationInfo = paginatedResponse.PaginationInfo;
        return View(ViewPath.DataTableView, paginatedResponse.Data);
    }

    /// <summary>
    ///     Hành động để hiển thị view tạo mới
    /// </summary>
    public virtual async Task<IActionResult> Create()
    {
        var data = GetModelData();

        if (data != null)
        {
            var isConfig = await ConfigCreateView(data);

            if (isConfig) return View(ViewPath.FormView, data);
        }

        SetMessage(Message.DataNull);
        return RedirectToIndex();
    }

    /// <summary>
    ///     Hành động để hiển thị view chỉnh sửa
    /// </summary>
    public virtual async Task<IActionResult> Edit()
    {
        var data = GetModelData();

        if (data != null)
        {
            var isConfig = await ConfigEditView(data);

            if (isConfig) return View(ViewPath.FormView, data);
        }

        SetMessage(Message.DataNull);
        return RedirectToIndex();
    }

    #endregion

    #region Process Action

    /// <summary>
    ///     Hành động để tạo mới một bản ghi
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Create(TRequest data)
    {
        return await ProcessActionAsync(Action.AddAsync, data);
    }


    /// <summary>
    ///     Hành động để cập nhật một bản ghi
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Edit(TRequest data)
    {
        return await ProcessActionAsync(Action.UpdateAsync, data);
    }


    /// <summary>
    ///     Xử lý hành động bất đồng bộ
    /// </summary>
    public virtual async Task<IActionResult> ProcessActionAsync(string action, TRequest? data)
    {
        // Lấy ID người dùng hiện tại
        var userId = GetCurrentUserId();

        // Kiểm tra nếu ID người dùng hoặc dữ liệu là null, đặt một thông báo và chuyển hướng về trang chính
        if (userId == null || data == null)
        {
            SetMessage(userId == null ? Message.UserEmpty : Message.DataNull);
            return RedirectToIndex();
        }

        // Cấu hình dữ liệu request
        ConfigRequestData(data);

        // Kiểm tra tính hợp lệ của dữ liệu và ModelState, quay trở lại view form nếu không hợp lệ
        if ((!await Valid(data) || !ModelState.IsValid) && action != Action.DeleteAsync)
        {
            // Thiết lập view với dữ liệu
            var isConfig = action == Action.AddAsync
                ? await ConfigCreateView(data)
                : await ConfigEditView(data);

            // Kiểm tra và thiết lập view nếu dữ liệu không thành công
            if (isConfig)
            {
                SetMessage(Message.DataNotValid);
                return View(ViewPath.FormView, data);
            }

            //Thiết lập view không thành công
            SetMessage(Message.DataNull);
            return RedirectToIndex();
        }

        // Thực hiện hành động được chỉ định trên dữ liệu và nhận một phản hồi thông báo
        var messageResponse = await _mainServices.ActionAsync(action, data, userId);

        // Đặt thông báo dựa trên phản hồi và chuyển hướng về trang chính
        SetMessage(messageResponse);
        return RedirectToIndex();
    }

    #endregion

    #region Custom Functions

    /// <summary>
    ///     Lấy response từ dịch vụ
    /// </summary>
    private protected virtual async Task<TRequest?> GetRequest(string? id)
    {
        return await _mainServices.GetRequestAsync(id);
    }

    /// <summary>
    ///     Lấy danh sách các response từ dịch vụ
    /// </summary>
    private protected virtual async Task<List<TResponse>?> GetListResponse()
    {
        return await _mainServices.GetListAsync(x => !x.IsDeleted);
    }


    /// <summary>
    ///     Lấy dữ liệu phân trang dựa trên danh sách
    /// </summary>
    private protected virtual PaginatedResponse<TResponse> GetPaginatedResponse(List<TResponse> listResponse)
    {
        return _mainServices.GetPaginated(listResponse, GetPaginationInfo());
    }

    /// <summary>
    ///     Lấy model trong sessionStorage.
    /// </summary>
    private TRequest? GetModelData()
    {
        return GetSessionStorage<TRequest>(SessionType.ModelData);
    }

    /// <summary>
    ///     Lấy model trong sessionStorage.
    /// </summary>
    private void SetModelData(TRequest data)
    {
        SetSessionStorage(SessionType.ModelData, data.ToJson());
    }

    /// <summary>
    ///     Cấu hình dữ liệu request
    /// </summary>
    private protected virtual void ConfigRequestData(TRequest data)
    {
    }


    /// <summary>
    ///     Cấu hình view create request
    /// </summary>
    private protected virtual Task<bool> ConfigCreateView(TRequest data)
    {
        return Task.FromResult(true);
    }

    /// <summary>
    ///     Cấu hình view edit request
    /// </summary>
    private protected virtual Task<bool> ConfigEditView(TRequest data)
    {
        return Task.FromResult(true);
    }


    /// <summary>
    ///     Kiểm tra tính hợp lệ của dữ liệu
    /// </summary>
    private protected virtual Task<bool> Valid(TRequest data)
    {
        return Task.FromResult(true);
    }

    #endregion
}