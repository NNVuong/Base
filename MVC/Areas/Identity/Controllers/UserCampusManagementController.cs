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
public class UserCampusManagementController : ManagementController<UserCampus, UserCampusRequest, UserCampusResponse>
{
    private const string Title = "Quản lý campus của người dùng";
    private readonly ICoreServices<Campus, CampusRequest, CampusResponse> _campusServices;
    private readonly ICoreServices<UserCampus, UserCampusRequest, UserCampusResponse> _userCampusServices;
    private readonly ICoreServices<User, UserRequest, UserResponse> _userServices;

    public UserCampusManagementController(
        ICoreServices<UserCampus, UserCampusRequest, UserCampusResponse> userCampusServices,
        ICoreServices<User, UserRequest, UserResponse> userServices,
        ICoreServices<Campus, CampusRequest, CampusResponse> campusServices
    ) : base(Title, AreaName.Identity, userCampusServices)
    {
        _userCampusServices = userCampusServices;
        _userServices = userServices;
        _campusServices = campusServices;
    }

    private protected override async Task<List<UserCampusResponse>?> GetListResponse()
    {
        return await _userCampusServices
            .GetListAsync(
                x => !x.IsDeleted,
                new List<Expression<Func<UserCampus, dynamic?>>>
                {
                    x => x.User,
                    x => x.Campus
                });
    }

    private protected override async Task<bool> ConfigCreateView(UserCampusRequest data)
    {
        return await GetSelect();
    }

    private protected override async Task<bool> ConfigEditView(UserCampusRequest data)
    {
        return await GetSelect($"{data.UserId}");
    }

    private protected override async Task<bool> Valid(UserCampusRequest data)
    {
        var listData = await _userCampusServices.GetListAsync();

        return listData != null &&
               listData
                   .Where(x => !x.IsDeleted)
                   .All(x => x.UserId != data.UserId || x.CampusId != data.CampusId);
    }

    private async Task<bool> GetSelect()
    {
        var users = await _userServices.GetListAsync(x => !x.IsDeleted);
        var campuses = await _campusServices.GetListAsync(x => !x.IsDeleted);

        if (users == null || campuses == null) return false;

        ViewData["Users"] = new SelectList(users.Select(x => new { Value = x.Id, Text = x.Email }), "Value", "Text");
        ViewData["Campuses"] =
            new SelectList(campuses.Select(x => new { Value = x.Id, Text = x.Name }), "Value", "Text");

        return true;
    }

    private async Task<bool> GetSelect(string userId)
    {
        var users = await _userServices.GetElementAsync(x => x.Id.ToString() == userId && !x.IsDeleted);
        var campuses = await _campusServices.GetListAsync(x => !x.IsDeleted);

        if (users == null || campuses == null) return false;

        ViewData["Users"] = new SelectList(new List<dynamic> { new { Value = users.Id, Text = users.Email } }, "Value",
            "Text");
        ViewData["Campuses"] =
            new SelectList(campuses.Select(x => new { Value = x.Id, Text = x.Name }), "Value", "Text");

        return true;
    }
}