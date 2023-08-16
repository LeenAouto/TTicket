using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TTicket.Abstractions.DAL;
using TTicket.Models;

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
                return await _context.User.SingleOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<User> GetByUsername(string username)
        {
            try
            {
                return await _context.User.SingleOrDefaultAsync(u => u.Username == username);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<User> GetByEmail(string email)
        {
            try
            {
                return await _context.User.SingleOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<User> GetByMobileNumber(string number)
        {
            try
            {
                return await _context.User.SingleOrDefaultAsync(u => u.MobilePhone == number);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            try
            {
                return await _context.User.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllByUserType(byte type)
        {
            try
            {
                return await _context.User.
                    Where(u => (byte)u.TypeUser == type).
                    ToListAsync();
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

        public async Task<bool> IsValidUserId(Guid id)
        {
            try
            {
                return await _context.User.AnyAsync(u => u.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<bool> IsClient(Guid id)
        {
            try
            {
                return await _context.User.AnyAsync(u => u.Id == id && u.TypeUser == UserType.Client);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<bool> IsSupport(Guid id)
        {
            try
            {
                return await _context.User.AnyAsync(u => u.Id == id && u.TypeUser == UserType.Support);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
