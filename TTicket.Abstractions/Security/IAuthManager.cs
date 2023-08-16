using TTicket.Models.UserManagementModels;

namespace TTicket.Abstractions.Security
{
    public interface IAuthManager
    {
        Task<AuthModel> Register(RegisterViewModel model); //Register
        Task<AuthModel> Login(LoginViewModel model); //Log in

        bool IsValidPassword(string password);
        bool IsValidMobileNumber(string number);
    }
}
