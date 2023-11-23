using System.Linq.Expressions;
using DataBase.Entities;
using DataTransferObjects.Request;
using DataTransferObjects.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Base;
using Services.Interfaces;
using Utilities.Constants;

namespace MVC.Areas.Identity.Controllers;

[Area(AreaName.Identity)]
[Authorize(Policy = PolicyType.IdentityArea)]
public class UserRoleManagementController : ManagementController<UserRole, UserRoleRequest, UserRoleResponse>
{
    private const string Title = "Quản lý quyền truy cập của người dùng";
    private readonly ICoreServices<Role, RoleRequest, RoleResponse> _roleServices;
    private readonly ICoreServices<UserRole, UserRoleRequest, UserRoleResponse> _userRoleServices;
    private readonly ICoreServices<User, UserRequest, UserResponse> _userServices;

    public UserRoleManagementController(
        ICoreServices<UserRole, UserRoleRequest, UserRoleResponse> userRoleServices,
        ICoreServices<User, UserRequest, UserResponse> userServices,
        ICoreServices<Role, RoleRequest, RoleResponse> roleServices
    )
        : base(Title, AreaName.Identity, userRoleServices)
    {
        _userRoleServices = userRoleServices;
        _userServices = userServices;
        _roleServices = roleServices;
    }

    private protected override async Task<List<UserRoleResponse>?> GetListResponse()
    {
        return await _userRoleServices.GetListAsync(
            x => !x.IsDeleted,
            new List<Expression<Func<UserRole, dynamic?>>>
            {
                x => x.User,
                x => x.Role
            });
    }

    private protected override async Task<bool> ConfigCreateView(UserRoleRequest data)
    {
        return await GetSelect();
    }

    private protected override async Task<bool> ConfigEditView(UserRoleRequest data)
    {
        return await GetSelect($"{data.UserId}");
    }

    private protected override async Task<bool> Valid(UserRoleRequest data)
    {
        var listData = await _userRoleServices.GetListAsync();

        return listData != null &&
               listData
                   .Where(x => !x.IsDeleted)
                   .All(x => x.UserId != data.UserId || x.RoleId != data.RoleId);
    }


    private async Task<bool> GetSelect()
    {
        var users = await _userServices.GetListAsync(x => !x.IsDeleted);
        var roles = await _roleServices.GetListAsync(x => !x.IsDeleted);

        if (users == null || roles == null) return false;

        ViewData["Users"] = new SelectList(users.Select(x => new { Value = x.Id, Text = x.Email }), "Value", "Text");
        ViewData["Roles"] = new SelectList(roles.Select(x => new { Value = x.Id, Text = x.Name }), "Value", "Text");

        return true;
    }

    private async Task<bool> GetSelect(string userId)
    {
        var users = await _userServices.GetElementAsync(x => x.Id.ToString() == userId && !x.IsDeleted);
        var roles = await _roleServices.GetListAsync(x => !x.IsDeleted);

        if (users == null || roles == null) return false;

        ViewData["Users"] = new SelectList(new List<dynamic> { new { Value = users.Id, Text = users.Email } }, "Value",
            "Text");
        ViewData["Roles"] = new SelectList(roles.Select(x => new { Value = x.Id, Text = x.Name }), "Value", "Text");

        return true;
    }
}