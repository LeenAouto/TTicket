using TTicket.Models;
using TTicket.Models.UserManagementModels;

namespace TTicket.DAL.Interfaces
{
    public interface IUserManager
    {
        Task<User> Get(Guid id);
        Task<User> GetByUsername(string username);
        Task<User> GetByEmail(string email);
        Task<User> GetByMobileNumber(string number);

        Task<IEnumerable<User>> GetAll();
        Task<IEnumerable<User>> GetAllByUserType(byte type);

        Task<User> Add(User user);
        User Update(User user);
        User Delete(User user);
        Task<bool> IsValidUserId(Guid id);

    }
}
