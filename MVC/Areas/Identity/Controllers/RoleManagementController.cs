using DataBase.Entities;
using DataTransferObjects.Request;
using DataTransferObjects.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Base;
using Services.Interfaces;
using Utilities.Constants;

namespace MVC.Areas.Identity.Controllers;

[Area(AreaName.Identity)]
[Authorize(Policy = PolicyType.IdentityArea)]
public class RoleManagementController : ManagementController<Role, RoleRequest, RoleResponse>
{
    private const string Title = "Quản lý quyền truy cập";
    private readonly ICoreServices<Role, RoleRequest, RoleResponse> _roleServices;

    public RoleManagementController(ICoreServices<Role, RoleRequest, RoleResponse> roleServices)
        : base(Title, AreaName.Identity, roleServices)
    {
        _roleServices = roleServices;
    }

    private protected override void ConfigRequestData(RoleRequest data)
    {
        base.ConfigRequestData(data);
        data.Name = data.Name.Trim().ToUpper();
    }

    private protected override async Task<bool> Valid(RoleRequest data)
    {
        var listData = await _roleServices.GetListAsync();

        return listData != null && listData
            .Where(x => !x.IsDeleted)
            .All(x => x.Name != data.Name);
    }
}