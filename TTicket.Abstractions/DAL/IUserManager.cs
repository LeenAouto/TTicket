using TTicket.Models;
using TTicket.Models.RequestModels;
using TTicket.Models.UserManagementModels;

namespace TTicket.Abstractions.DAL
{
    public interface IUserManager
    {
        Task<User> Get(UserRequestModel model);
        Task<IEnumerable<User>> GetList(UserListRequestModel model);
        Task<User> Add(User user);
        User Update(User user);
        User Delete(User user);



        //Task<bool> IsValidUserId(Guid id);
        //Task<bool> IsClient(Guid id);
        //Task<bool> IsSupport(Guid id);
    }
}
