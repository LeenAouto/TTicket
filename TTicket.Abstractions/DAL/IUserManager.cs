using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;

namespace TTicket.Abstractions.DAL
{
    public interface IUserManager
    {
        Task<UserModel> Get(Guid id);
        Task<SecureUserModel> GetByIdentity(UserRequestModel model);
        Task<PagedResponse<UserModel>> GetList(UserListRequestModel model);
        Task<UserModel> Add(SecureUserModel user);
        Task<UserModel> Update(SecureUserModel user);
        Task<UserModel> Delete(UserModel user);
    }
}
