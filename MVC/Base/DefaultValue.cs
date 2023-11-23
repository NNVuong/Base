using Utilities.Constants;

namespace MVC.Base;

public static class DefaultValue
{
    public const string Area = AreaName.Guest;
    public const string Controller = "Home";
    public const string Action = "Index";

    public const string ErrorPath = $"{Area}/{Controller}/Error";
    public const string AccessDeniedPath = $"{Area}/{Controller}/AccessDenied";
    public const string LoginPath = $"{AreaName.Identity}/Auth/Login";
}