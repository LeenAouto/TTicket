using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;

namespace TTicket.DAL.Managers
{
    public class UserManager : IUserManager
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserManager> _logger;

        public UserManager(ApplicationDbContext context, ILogger<UserManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User> Get(Guid id)
        {
            try
            {
                return await _context.User.Where(u => u.Id == id).SingleOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
        public async Task<User> GetByIdentity(UserRequestModel model)
        {
            try
            {
                return await _context.User.
                Where(u => (u.Id == model.Id || u.Username == model.Identity || u.Email == model.Identity || u.MobilePhone == model.Identity) 
                        && (u.TypeUser == model.TypeUser || model.TypeUser == null)).
                FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
        public async Task<IEnumerable<UserModel>> GetList(UserListRequestModel model)
        {
            try
            {
                var skip = (model.PageNumber - 1) * model.PageSize;
                var users = await _context.User.
                    Where(u => (u.Username == model.Username || model.Username == null)
                            && (u.Email == model.Email || model.Email == null)
                            && (u.MobilePhone == model.MobilePhone || model.MobilePhone == null)
                            && (u.FirstName == model.FirstName || model.FirstName == null)
                            && (u.LastName == model.LastName || model.LastName == null)
                            && (u.TypeUser == model.TypeUser || model.TypeUser == null)
                            && (u.StatusUser == model.StatusUser || model.StatusUser == null)
                            ).
                    Skip(skip).
                    Take(model.PageSize).
                    Select(u => new UserModel(u, u.TypeUser == UserType.Client ? _context.Ticket.Count(t => t.UserId == u.Id) : null)).
                    ToListAsync();
                return users;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<User> Add(User user)
        {
            try
            {
                await _context.User.AddAsync(user);
                _context.SaveChanges();
                return user;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public User Update(User user)
        {
            try
            {
                _context.User.Update(user);
                _context.SaveChanges();
                return user;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public User Delete(User user)
        {
            try
            {
                _context.User.Remove(user);
                _context.SaveChanges();
                return user;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
