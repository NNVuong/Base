using System.Diagnostics;
using DataTransferObjects.Response;
using Microsoft.AspNetCore.Mvc;
using MVC.Base;
using Utilities.Constants;

namespace MVC.Areas.Guest.Controllers;

[Area(AreaName.Guest)]
public class HomeController : CustomController
{
    private const string Title = "Trang chủ";

    public HomeController() : base(Title, AreaName.Guest)
    {
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult AccessDenied(string? content)
    {
        return View(new AccessDeniedResponse
        {
            Message = content
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string? content)
    {
        return View(new ErrorResponse
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            Message = content
        });
    }
}