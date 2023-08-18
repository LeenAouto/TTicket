using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using TTicket.Abstractions.DAL;
using TTicket.Abstractions.Security;
using TTicket.DAL.Managers;
using TTicket.Models;
using TTicket.Models.RequestModels;
using TTicket.Models.UserManagementModels;
using TTicket.Security.Settings;

namespace TTicket.Security
{
    public class AuthManager : IAuthManager
    {
        private readonly JWT _jwt;
        private readonly ILogger<UserManager> _logger;
        private readonly IPasswordHasher _hasher;
        private readonly IUserManager _userManager;
        public AuthManager(IOptions<JWT> jwt, ILogger<UserManager> logger, IUserManager userManager, IPasswordHasher hasher)
        {
            _jwt = jwt.Value;
            _logger = logger;
            _userManager = userManager;
            _hasher = hasher;
        }

        public async Task<AuthModel> Register(RegisterViewModel model)
        {
            try
            {
                var request = new UserRequestModel
                {
                    Identity = model.Username
                };

                if (await _userManager.Get(request) != null)
                    return new AuthModel { Message = "Username is already registered" };

                request.Identity = model.Email;
                if (await _userManager.Get(request) != null)
                    return new AuthModel { Message = "Email is already registered" };

                request.Identity = model.MobilePhone;
                if (await _userManager.Get(request) != null)
                    return new AuthModel { Message = "The phone number is already used in another account" };

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
                    Username = resultUser.Username.ToLower(),
                    Email = resultUser.Email.ToLower(),
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

                var request = new UserRequestModel { Identity = model.Identity };

                var user = await _userManager.Get(request);

                if (user is null || !_hasher.Verify(user.Password, model.Password))
                {
                    authModel.Message = "Incorrect credentials!";
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

        public bool IsValidUserName(string userName)
        {
            if(userName.Length < 1)
                return false;

            if (!Regex.IsMatch(userName, @"^[a-zA-Z0-9!#$%^&*()_+{}\[\]:;<>,.?~]+$"))
                return false;

            return true;
        }

        public bool IsValidPassword(string password)
        {
            if (password.Length < 8)
                return false;

            if (!Regex.IsMatch(password, @"^[a-zA-Z0-9!@#$%^&*()_+{}\[\]:;<>,.?~]+$"))
                return false;

            if (!Regex.IsMatch(password, @"\d"))
                return false;

            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+{}\[\]:;<>,.?~]"))
                return false;

            return true;
        }

        public bool IsValidMobileNumber(string number)
        {
            if (string.IsNullOrEmpty(number))
                return false;

            return Regex.IsMatch(number, @"^0+5+\d{8}$");
        }

        public bool IsValidEmailAddress(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
                return false;

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(emailAddress, pattern);
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
