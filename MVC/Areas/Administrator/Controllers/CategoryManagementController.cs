using DataBase.Entities;
using DataTransferObjects.Request;
using DataTransferObjects.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Base;
using NuGet.Protocol;
using Services.Interfaces;
using Utilities.Constants;

namespace MVC.Areas.Administrator.Controllers;

[Area(AreaName.Administrator)]
[Authorize(Policy = PolicyType.AdministratorArea)]
public class CategoryManagementController : ManagementController<Category, CategoryRequest, CategoryResponse>
{
    private const string Title = "Quản lý danh mục";
    private readonly ICoreServices<Category, CategoryRequest, CategoryResponse> _categoryServices;

    public CategoryManagementController(ICoreServices<Category, CategoryRequest, CategoryResponse> categoryServices)
        : base(Title, AreaName.Administrator, categoryServices)
    {
        _categoryServices = categoryServices;
    }

    public async Task<IActionResult> Details(string? id)
    {
        var data = await _categoryServices.GetElementAsync(id);

        if (data == null)
        {
            SetMessage(Message.DataNull);
            return RedirectToIndex();
        }

        SetSessionStorage(SessionType.ModelData, data.ToJson());
        return RedirectToAction("Index", "CategoryDetailsManagement", new { area = AreaName.Administrator });
    }

    private protected override void ConfigRequestData(CategoryRequest data)
    {
        base.ConfigRequestData(data);
        data.Name = data.Name.Trim();
    }

    private protected override async Task<bool> Valid(CategoryRequest data)
    {
        var listData = await _categoryServices.GetListAsync();

        return listData != null &&
               listData
                   .Where(x => !x.IsDeleted)
                   .All(x => x.Name != data.Name);
    }
}