using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using TTicket.DAL.Interfaces;
using TTicket.DAL.Managers;
using TTicket.Models;
using TTicket.Models.UserManagementModels;
using TTicket.Security.Interfaces;
using TTicket.Services.Auth.Interfaces;
using TTicket.Settings;

namespace TTicket.Services.Auth.Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly JWT _jwt;
        private readonly ILogger<UserManager> _logger;
        private readonly IPasswordHasher _hasher;
        private readonly IUserManager _userManager;
        public AuthManager(JWT jwt, ILogger<UserManager> logger, IUserManager userManager, IPasswordHasher hasher)
        {
            _jwt = jwt;
            _logger = logger;
            _userManager = userManager;
            _hasher = hasher;
        }

        public async Task<AuthModel> Register(RegisterViewModel model)
        {
            try
            {
                if (await _userManager.GetByEmail(model.Email) != null)
                    return new AuthModel { Message = "Email is already registered" };

                if (await _userManager.GetByUsername(model.Username) != null)
                    return new AuthModel { Message = "Username is already registered" };

                if (await _userManager.GetByMobileNumber(model.MobilePhone) != null)
                    return new AuthModel { Message = "The phone number is already used in another account" };

                //TODO: Use automapper here to map ApplicationUser to the RegistrModel
                var user = new User
                {
                    Username = model.Username,
                    Password = _hasher.Hash(model.Password),
                    Email = model.Email,
                    MobilePhone = model.MobilePhone,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    TypeUser = model.TypeUser == 2 ? UserType.Support : UserType.Client,
                    StatusUser = UserStatus.Active
                };

                var resultUser = await _userManager.Add(user);

                var jwtSecurityToken = CreateJwtToken(user);

                return new AuthModel
                {
                    Id = resultUser.Id,
                    Username = resultUser.Username,
                    Email = resultUser.Email,
                    MobilePhone = resultUser.MobilePhone,
                    TypeUser = (byte)resultUser.TypeUser,
                    StatusUser = (byte)resultUser.StatusUser,

                    Message = "Success",
                    IsAuthenticated = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    ExpiresOn = jwtSecurityToken.ValidTo
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }        
        }
        public async Task<AuthModel> Login(LoginViewModel model)
        {
            try
            {
                var authModel = new AuthModel();
                var user = new User();

                if (new EmailAddressAttribute().IsValid(model.Identity))
                {
                    user = await _userManager.GetByEmail(model.Identity);
                }
                else if (Regex.IsMatch(model.Identity, @"^0+5+\d{8}$"))
                {
                    user = await _userManager.GetByMobileNumber(model.Identity);
                }
                else
                {
                    user = await _userManager.GetByUsername(model.Identity);
                }

                if (user is null || !_hasher.Verify(user.Password, model.Password))
                {
                    authModel.Message = "Email or Password is incorrect!";
                    return authModel;
                }

                var jwtSecurityToken = CreateJwtToken(user);

                authModel.Id = user.Id;
                authModel.Username = user.Username;
                authModel.Email = user.Email;
                authModel.MobilePhone = user.MobilePhone;
                authModel.TypeUser = (byte)user.TypeUser;
                authModel.StatusUser = (byte)user.StatusUser;

                authModel.Message = "Success";
                authModel.IsAuthenticated = true;
                authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authModel.ExpiresOn = jwtSecurityToken.ValidTo;

                return authModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        private JwtSecurityToken CreateJwtToken(User user)
        {
            var userClaims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
