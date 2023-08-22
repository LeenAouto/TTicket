using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
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

        public async Task<UserModel> Get(Guid id)
        {
            try
            {
                return await _context.User.
                    Where(u => u.Id == id).
                    Select(u => new UserModel(u, u.TypeUser == UserType.Client ? _context.Ticket.Count(t => t.UserId == u.Id) : null)).
                    SingleOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
        public async Task<UserModel> GetByIdentity(UserRequestModel model)
        {
            try
            {
                return await _context.User.
                Where(u => (u.Id == model.Id || u.Username == model.Identity || u.Email == model.Identity || u.MobilePhone == model.Identity)
                        && (u.TypeUser == model.TypeUser || model.TypeUser == null)).
                Select(u => new UserModel(u, u.TypeUser == UserType.Client ? _context.Ticket.Count(t => t.UserId == u.Id) : null)).
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
                    OrderBy(u => u.TypeUser).
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

        public async Task<UserModel> Add(UserModel user)
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
                return new UserModel(u);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<UserModel> Update(UserModel user)
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

                return new UserModel(u);
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

                return new UserModel(u);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
