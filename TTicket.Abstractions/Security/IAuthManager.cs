using TTicket.Models.UserManagementModels;

namespace TTicket.Abstractions.Security
{
    public interface IAuthManager
    {
        Task<RegisterResponse> RegisterClient(RegisterViewModel model); //Register 
        Task<RegisterResponse> RegisterSupport(RegisterViewModel model); //Register 
        Task<LoginResponse> Login(LoginViewModel model); //Log in

        bool IsValidUserName(string userName);
        bool IsValidPassword(string password);
        bool IsValidMobileNumber(string number);
        bool IsValidEmailAddress(string emailAddress);
    }
}
