namespace Utilities.Constants;

public static class Action
{
    public const string AddAsync = Add + Async;

    public const string UpdateAsync = Update + Async;

    public const string DeleteAsync = Delete + Async;

    public const string CopyAsync = Copy + Async;

    public const string HardDeleteAsync = HardDelete + Async;

    public const string LogoutFeid = "https://feid.ptudev.net/connect/endsession?id_token_hint=";

    #region Private

    private const string Async = "Async";
    private const string Add = "Add";
    private const string Update = "Update";
    private const string Delete = "Delete";
    private const string Copy = "Copy";
    private const string HardDelete = "HardDelete";

    #endregion
}