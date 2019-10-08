
namespace Parsnet
{
    public enum LoginStatus
    {
        InvalidUsername,
        IsLockedOut,
        IsNotApproved,
        TryLater,
        InvalidPassword,
        IsValid,
        ValidationError
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

    public enum ChangePasswordResult
    {
        InvalidOldPassword,
        Success,
        Fail
    }

    public class Message
    {
        public Commands Command { get; set; }
        public object Data { get; set; }
        public string SystemId { get; set; }
        public string Version { get; set; }
    }
}
