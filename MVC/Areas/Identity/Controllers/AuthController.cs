using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using DataBase.Entities;
using DataTransferObjects.Request;
using DataTransferObjects.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Utilities.Constants;
using Action = Utilities.Constants.Action;

namespace MVC.Areas.Identity.Controllers;

[Area(AreaName.Identity)]
public class AuthController : Controller
{
    private readonly ICoreServices<Role, RoleRequest, RoleResponse> _roleServices;
    private readonly ICoreServices<UserCampus, UserCampusRequest, UserCampusResponse> _userCampusServices;
    private readonly ICoreServices<UserRole, UserRoleRequest, UserRoleResponse> _userRoleServices;
    private readonly ICoreServices<User, UserRequest, UserResponse> _userServices;

    public AuthController(
        ICoreServices<User, UserRequest, UserResponse> userServices,
        ICoreServices<Role, RoleRequest, RoleResponse> roleServices,
        ICoreServices<UserRole, UserRoleRequest, UserRoleResponse> userRoleServices,
        ICoreServices<UserCampus, UserCampusRequest, UserCampusResponse> userCampusServices
    )
    {
        _userServices = userServices;
        _roleServices = roleServices;
        _userRoleServices = userRoleServices;
        _userCampusServices = userCampusServices;
    }

    /// <summary>
    ///     Khởi tạo quá trình đăng nhập.
    /// </summary>
    public async Task Login(string? returnUrl)
    {
        try
        {
            // Thực hiện đăng nhập thông qua OpenIdConnect
            await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("FeidLoginResponse", "Auth", new { returnUrl })
            });
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu có
            RedirectToAction("Error", "Home", new { area = AreaName.Guest, content = ex.Message });
        }
    }

    /// <summary>
    ///     Thực hiện đăng xuất người dùng.
    /// </summary>
    public IActionResult Logout()
    {
        // Thực hiện đăng xuất và điều hướng đến trang AccessDenied nếu không có thông tin đăng nhập
        var claimIdToken = User.Claims.FirstOrDefault(x => x.Type == UserClaimTypes.TokenId);
        var sessionIdToken = HttpContext.Session.GetString(SessionType.TokenId);

        if (claimIdToken == null && string.IsNullOrEmpty(sessionIdToken))
            return RedirectToAction("AccessDenied", "Home", new { area = AreaName.Guest });

        var idToken = claimIdToken == null
            ? sessionIdToken
            : claimIdToken.Value;

        var url = $"{Action.LogoutFeid}{idToken}";

        return Redirect(url);
    }

    /// <summary>
    ///     Xử lý phản hồi sau khi đăng nhập FEID thành công.
    /// </summary>
    public async Task<IActionResult> FeidLoginResponse(string? returnUrl)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var idToken = await HttpContext.GetTokenAsync("id_token");

        HttpContext.Session.SetString(SessionType.TokenId, idToken ?? "");

        if (accessToken == null || idToken == null)
            /* Xác thực lỗi */
            return RedirectToAction("Error", "Home", new { area = AreaName.Guest, content = Message.Warning });

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

        var claimEmail = jwt.Claims.FirstOrDefault(x => x.Type == UserClaimTypes.Email);

        if (claimEmail == null)
            /* Không tìm thấy Email */
            return RedirectToAction("Error", "Home", new { area = AreaName.Guest, content = Message.Warning });

        var userEmail = claimEmail.Value;

        if (string.IsNullOrEmpty(userEmail))
            /* Không tìm thấy Email */
            return RedirectToAction("Error", "Home", new { area = AreaName.Guest, content = Message.Warning });

        var user = await _userServices.GetElementAsync(x => !x.IsDeleted && x.Email == userEmail.ToLower()) ??
                   await CreateUser(userEmail);

        if (user == null)
            return RedirectToAction("Error", "Home", new { area = AreaName.Guest, content = Message.Failure });

        var userRole = await _userRoleServices.GetListAsync(
            x => !x.IsDeleted && x.UserId == user.Id && !x.Role.IsDeleted,
            new List<Expression<Func<UserRole, dynamic?>>>
            {
                x => x.Role
            });

        if (userRole == null || userRole.Count == 0)
        {
            /* Không tìm thấy UserRole trong Database */

            var dataEntryRole =
                await _roleServices.GetElementAsync(x => !x.IsDeleted && x.Name == UserRoleType.DataEntry)
                ?? await CreateRole(UserRoleType.DataEntry);

            if (dataEntryRole == null)
                return RedirectToAction("Error", "Home", new { area = AreaName.Guest, content = Message.Failure });

            userRole = await CreateUserRole(user.Id, dataEntryRole.Id);

            if (userRole == null || userRole.Count == 0)
                return RedirectToAction("Error", "Home", new { area = AreaName.Guest, content = Message.Failure });
        }

        var userCampus = await _userCampusServices.GetListAsync(
            x => !x.IsDeleted && x.UserId == user.Id && !x.Campus.IsDeleted,
            new List<Expression<Func<UserCampus, dynamic?>>>
            {
                x => x.Campus
            });

        if (userCampus == null || userCampus.Count == 0)
            /* Không tìm thấy Campus trong Database */
            return RedirectToAction("Error", "Home", new { area = AreaName.Guest, content = Message.UserCampusEmpty });


        await HttpContext.SignInAsync(GetClaimsResponse(user, userRole, userCampus, accessToken, idToken));

        return string.IsNullOrEmpty(returnUrl)
            ? RedirectToAction("Index", "Home", new { area = AreaName.Guest })
            : Redirect(returnUrl);
    }

    /// <summary>
    ///     Xử lý phản hồi sau khi đăng xuất FEID.
    /// </summary>
    public async Task<IActionResult> FeidLogoutResponse()
    {
        // Thực hiện xử lý phản hồi sau khi đăng xuất FEID
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home", new { area = AreaName.Guest });
    }

    /// <summary>
    ///     Tạo một người dùng mới nếu chưa tồn tại.
    /// </summary>
    private async Task<UserResponse?> CreateUser(string email)
    {
        // Thực hiện tạo người dùng mới

        email = email.ToLower();

        var user = await _userServices.GetElementAsync(x => !x.IsDeleted && x.Email == email);

        if (user != null) return user;

        var newUser = new UserRequest
        {
            Id = Guid.NewGuid(),
            Name = email.Split("@")[0].ToUpper(),
            Email = email
        };

        await _userServices.ActionAsync(Action.AddAsync, newUser, $"{Guid.Empty}");

        return await _userServices.GetElementAsync(x => !x.IsDeleted && x.Email == email);
    }

    /// <summary>
    ///     Tạo một vai trò mới nếu chưa tồn tại.
    /// </summary>
    private async Task<RoleResponse?> CreateRole(string name)
    {
        // Thực hiện tạo vai trò mới

        name = name.ToUpper();

        var role = await _roleServices.GetElementAsync(x => !x.IsDeleted && x.Name == name);

        if (role != null) return role;


        var newRole = new RoleRequest
        {
            Name = name,
            Description = name
        };

        await _roleServices.ActionAsync(Action.AddAsync, newRole, $"{Guid.Empty}");

        return await _roleServices.GetElementAsync(x => !x.IsDeleted && x.Name == name);
    }

    /// <summary>
    ///     Tạo một phân quyền người dùng mới nếu chưa tồn tại.
    /// </summary>
    private async Task<List<UserRoleResponse>?> CreateUserRole(Guid userId, Guid roleId)
    {
        // Thực hiện tạo phân quyền người dùng mới
        var userRole = await _userRoleServices
            .GetListAsync(
                x => !x.IsDeleted && x.UserId == userId && x.RoleId == roleId,
                new List<Expression<Func<UserRole, dynamic?>>>
                {
                    x => x.Role
                });

        if (userRole is { Count: > 0 }) return userRole;

        var newUserRole = new UserRoleRequest
        {
            UserId = userId,
            RoleId = roleId
        };

        await _userRoleServices.ActionAsync(Action.AddAsync, newUserRole, $"{Guid.Empty}");

        return await _userRoleServices
            .GetListAsync(
                x => !x.IsDeleted && x.UserId == userId && x.RoleId == roleId,
                new List<Expression<Func<UserRole, dynamic?>>>
                {
                    x => x.Role
                });
    }

    /// <summary>
    ///     Xây dựng các chứng chỉ và tạo một ClaimsPrincipal cho việc xác thực người dùng.
    /// </summary>
    private static ClaimsPrincipal GetClaimsResponse(UserResponse user, IEnumerable<UserRoleResponse> userRole,
        IEnumerable<UserCampusResponse> userCampus, string accessToken, string idToken)
    {
        // Thực hiện xây dựng chứng chỉ và tạo ClaimsPrincipal
        var claims = new List<Claim>
        {
            new(UserClaimTypes.UserId, $"{user.Id}"),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(UserClaimTypes.AccessToken, accessToken),
            new(UserClaimTypes.TokenId, idToken)
        };

        claims.AddRange(userRole.Select(x => new Claim(ClaimTypes.Role, x.RoleName)));

        var userCampusResponses = userCampus.ToList();

        claims.AddRange(userCampusResponses.Select(x => new Claim(UserClaimTypes.CampusName, x.CampusName)));

        claims.AddRange(userCampusResponses.Select(x => new Claim(UserClaimTypes.CampusCode, x.CampusCode)));

        var claimsIdentity = new ClaimsIdentity(claims, UserClaimTypes.AuthenticationType);

        return new ClaimsPrincipal(claimsIdentity);
    }
}