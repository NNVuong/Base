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
public class CampusManagementController : ManagementController<Campus, CampusRequest, CampusResponse>
{
    private const string Title = "Quản lý campus";
    private readonly ICoreServices<Campus, CampusRequest, CampusResponse> _campusServices;

    public CampusManagementController(ICoreServices<Campus, CampusRequest, CampusResponse> campusServices)
        : base(Title, AreaName.Identity, campusServices)
    {
        _campusServices = campusServices;
    }

    private protected override void ConfigRequestData(CampusRequest data)
    {
        base.ConfigRequestData(data);
        data.Name = data.Name.Trim();
    }

    private protected override async Task<bool> Valid(CampusRequest data)
    {
        var listData = await _campusServices.GetListAsync();

        return listData != null && listData
            .Where(x => !x.IsDeleted)
            .All(x => x.Code != data.Code);
    }
}