using System.Linq.Expressions;
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
public class UserManagementController : ManagementController<User, UserRequest, UserResponse>
{
    private const string Title = "Quản lý người dùng";
    private readonly ICoreServices<Role, RoleRequest, RoleResponse> _roleServices;
    private readonly ICoreServices<User, UserRequest, UserResponse> _userServices;

    public UserManagementController(ICoreServices<User, UserRequest, UserResponse> userServices,
        ICoreServices<Role, RoleRequest, RoleResponse> roleServices)
        : base(Title, AreaName.Identity, userServices)
    {
        _userServices = userServices;
        _roleServices = roleServices;
    }

    private protected override async Task<List<UserResponse>?> GetListResponse()
    {
        return await _userServices.GetListAsync(
            x => !x.IsDeleted,
            new List<Expression<Func<User, dynamic?>>>
            {
                x => x.UserRole
            });
    }


    private protected override void ConfigRequestData(UserRequest data)
    {
        base.ConfigRequestData(data);
        data.Name = data.Name.Trim().ToUpper();
        data.Email = data.Email.Trim().ToLower();
    }

    private protected override async Task<bool> Valid(UserRequest data)
    {
        var listData = await _userServices.GetListAsync();

        return listData != null && listData
            .Where(x => !x.IsDeleted)
            .All(x => x.Email != data.Email);
    }
}