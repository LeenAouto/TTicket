using TTicket.Models;
using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;

namespace TTicket.Abstractions.DAL
{
    public interface IUserManager
    {
        Task<User> Get(Guid id);
        Task<User> GetByIdentity(UserRequestModel model);
        Task<IEnumerable<UserModel>> GetList(UserListRequestModel model);
        Task<User> Add(User user);
        User Update(User user);
        User Delete(User user);
    }
}
