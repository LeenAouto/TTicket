using TTicket.Models.UserManagementModels;

namespace TTicket.Services.Auth.Interfaces
{
    public interface IAuthManager
    {
        Task<AuthModel> Register(RegisterViewModel model); //Register
        Task<AuthModel> Login(LoginViewModel model); //Log in
    }
}
