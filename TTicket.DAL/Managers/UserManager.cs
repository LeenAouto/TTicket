using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;

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

        public async Task<UserModel> Get(Guid id)
        {
            try
            {
                var user = await _context.User.
                    Where(u => u.Id == id).
                    Select(u => new UserModel(u, u.TypeUser == UserType.Client ? _context.Ticket.Count(t => t.UserId == u.Id) : null)).
                    SingleOrDefaultAsync();

                return user;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<SecureUserModel> GetByIdentity(UserRequestModel model)
        {
            try
            {
                var user = await _context.User.
                Where(u => (u.Id == model.Id || u.Username == model.Identity || u.Email == model.Identity || u.MobilePhone == model.Identity)
                        && (u.TypeUser == model.TypeUser || model.TypeUser == null)).
                Select(u => new SecureUserModel(u)).
                FirstOrDefaultAsync();

                return user;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<PagedResponse<UserModel>> GetList(UserListRequestModel model)
        {
            try
            {
                var skip = (model.PageNumber - 1) * model.PageSize;

                var query = _context.User.
                    Where(u => (u.Username == model.Username || model.Username == null)
                            && (u.Email == model.Email || model.Email == null)
                            && (u.MobilePhone == model.MobilePhone || model.MobilePhone == null)
                            && (u.FirstName == model.FirstName || model.FirstName == null)
                            && (u.LastName == model.LastName || model.LastName == null)
                            && (u.TypeUser == model.TypeUser || model.TypeUser == null)
                            && (u.StatusUser == model.StatusUser || model.StatusUser == null));

                var totalCount = await query.CountAsync();

                var users = await query.
                    OrderBy(u => u.TypeUser).
                    Skip(skip).
                    Take(model.PageSize).
                    Select(u => new UserModel(u, u.TypeUser == UserType.Client ? _context.Ticket.Count(t => t.UserId == u.Id) : null)).
                    ToListAsync();

                return new PagedResponse<UserModel>(users, totalCount);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<UserModel> Add(SecureUserModel user)
        {
            try
            {
                var u = new User
                {
                    Username = user.Username,
                    Password = user.Password,
                    Email = user.Email,
                    MobilePhone = user.MobilePhone,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address,
                    TypeUser = user.TypeUser,
                    StatusUser = user.StatusUser
                };

                await _context.User.AddAsync(u);
                _context.SaveChanges();
                return new UserModel(u, u.TypeUser == UserType.Client ? _context.Ticket.Count(t => t.UserId == u.Id) : null);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<UserModel> Update(SecureUserModel user)
        {
            try
            {
                var u = await _context.User.Where(u => u.Id == user.Id).SingleAsync();

                u.Username = user.Username;
                u.Password = user.Password;
                u.Email = user.Email;
                u.MobilePhone = user.MobilePhone;
                u.FirstName = user.FirstName;
                u.LastName = user.LastName;
                u.DateOfBirth = user.DateOfBirth;
                u.Address = user.Address;
                u.TypeUser = user.TypeUser;
                u.StatusUser = user.StatusUser;



                _context.User.Update(u);
                _context.SaveChanges();

                return new UserModel(u, u.TypeUser == UserType.Client ? _context.Ticket.Count(t => t.UserId == u.Id) : null);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<UserModel> Delete(UserModel user)
        {
            try
            {
                var u = await _context.User.Where(u => u.Id == user.Id).SingleAsync();

                _context.User.Remove(u);
                _context.SaveChanges();

                return new UserModel(u, u.TypeUser == UserType.Client ? _context.Ticket.Count(t => t.UserId == u.Id) : null);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
