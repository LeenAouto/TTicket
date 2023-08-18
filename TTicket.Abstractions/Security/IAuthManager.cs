using TTicket.Models.UserManagementModels;

namespace TTicket.Abstractions.Security
{
    public interface IAuthManager
    {
        Task<AuthModel> Register(RegisterViewModel model); //Register
        Task<AuthModel> Login(LoginViewModel model); //Log in

        bool IsValidUserName(string userName);
        bool IsValidPassword(string password);
        bool IsValidMobileNumber(string number);
        bool IsValidEmailAddress(string emailAddress);
    }
}
