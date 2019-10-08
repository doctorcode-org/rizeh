using System.Reflection;
namespace Parsnet
{
    public delegate void delSendCommand(Commands cmd, object data);
    public delegate void delWriteError(MemberInfo info, string msg);
    public delegate void ExeptionOccuredHandler(string method, string data);
    public delegate void LossConnectionHandler(NetClient client);

    public enum ChangePasswordResult
    {
        InvalidOldPassword,
        Success,
        Fail
    }

    public enum Commands
    {
        Login,
        Signup,
        Confirm,
        RegisterSite,
        UpdateStatus,
        DeleteSite,
        UpdateSite,
        NextSite,
        UserWebsiteList,
        UserPayments,
        ProductList,
        AdminMsg,
        CheckVersion,
        Pays,
        ChangePassword,
        ResetPassword
    }

    public enum InsertSiteResult
    {
        Error,
        NoCredits,
        Success,
        InvalidCode,
        Duplicate
    }

    public enum LoginStatus
    {
        InvalidUsername,
        IsLockedOut,
        IsNotApproved,
        TryLater,
        InvalidPassword,
        IsValid,
        ValidationError,
        MultipeLogin
    }


}

