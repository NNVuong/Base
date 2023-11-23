using System.Linq.Expressions;
using DataBase.Entities;
using DataTransferObjects.Request;
using DataTransferObjects.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Base;
using Services.Interfaces;
using Utilities.Constants;

namespace MVC.Areas.Administrator.Controllers;

[Area(AreaName.Administrator)]
[Authorize(Policy = PolicyType.AdministratorArea)]
public class
    CategoryDetailsManagementController : ManagementController<CategoryDetails, CategoryDetailsRequest,
        CategoryDetailsResponse>
{
    private const string Title = "Quản lý chi tiết danh mục";

    private readonly ICoreServices<CategoryDetails, CategoryDetailsRequest, CategoryDetailsResponse>
        _categoryDetailsServices;

    public CategoryDetailsManagementController(
        ICoreServices<CategoryDetails, CategoryDetailsRequest, CategoryDetailsResponse> categoryDetailsServices)
        : base(Title, AreaName.Administrator, categoryDetailsServices)
    {
        _categoryDetailsServices = categoryDetailsServices;
    }

    private protected override async Task<List<CategoryDetailsResponse>?> GetListResponse()
    {
        return await _categoryDetailsServices.GetListAsync(
            x => !x.IsDeleted,
            new List<Expression<Func<CategoryDetails, dynamic?>>>
            {
                x => x.Category
            });
    }

    private protected override void ConfigRequestData(CategoryDetailsRequest data)
    {
        base.ConfigRequestData(data);
        data.Name = data.Name.Trim();
    }

    private protected override async Task<bool> Valid(CategoryDetailsRequest data)
    {
        var listData = await _categoryDetailsServices.GetListAsync();

        return listData != null &&
               listData.Where(x => !x.IsDeleted)
                   .All(x => x.Name != data.Name);
    }
}