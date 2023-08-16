using TTicket.Models;

namespace TTicket.Abstractions.DAL
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
        Task<bool> IsClient(Guid id);
        Task<bool> IsSupport(Guid id);
    }
}
