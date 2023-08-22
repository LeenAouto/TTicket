using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;

namespace TTicket.Abstractions.DAL
{
    public interface IUserManager
    {
        Task<UserModel> Get(Guid id);
        Task<UserModel> GetByIdentity(UserRequestModel model);
        Task<IEnumerable<UserModel>> GetList(UserListRequestModel model);
        Task<UserModel> Add(UserModel user);
        Task<UserModel> Update(UserModel user);
        Task<UserModel> Delete(UserModel user);
    }
}
