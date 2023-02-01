using AuthenticationSystem.Identity;
using AuthenticationSystem.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSystem.Repository
{
    public class UserServiceRepository : IUserServiceRepository
    {
        private readonly ApplicationSignInManager _signInManager;
        private readonly ApplicationUserManager _userManager;
        private readonly IJwtManagerRepository _jwtManager;
        public UserServiceRepository(ApplicationSignInManager signInManager,ApplicationUserManager userManager,IJwtManagerRepository jwtManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtManager = jwtManager;
        }
        public async Task<ApplicationUser> AuthenticateUser(string userName, string userPassword)
        {
            var userExist = await _userManager.FindByNameAsync(userName);
            var userVerification = await _signInManager.CheckPasswordSignInAsync(userExist, userPassword, false);
            if (!userVerification.Succeeded) return null;
            var roleUser = await _userManager.GetRolesAsync(userExist);
            userExist.Role = roleUser.FirstOrDefault();
            var  userTokenGenerated = _jwtManager.GenerateToken(userExist);

            // check the user refresh token or  add the user refresh token or update the token.
               return await AddUserRefreshToken(userTokenGenerated);
         }

        public async Task<bool> IsUnique(string userName)
        {
            var userExist = await _userManager.FindByNameAsync(userName);
            if (userExist == null) return true;
            return false;
        }

        public async Task<bool> RegisterUser(ApplicationUser userCredentials)
        {
            var user = await _userManager.CreateAsync(userCredentials,userCredentials.PasswordHash);
            if (!user.Succeeded) return false;
            await _userManager.AddToRoleAsync(userCredentials, userCredentials.Role);
            return true;
        }
        public async Task<ApplicationUser> AddUserRefreshToken(ApplicationUser user)
        {
            var userDetail = await _userManager.UpdateAsync(user);
            if (userDetail.Succeeded) return user;
            return null;
        }
        public async Task<ApplicationUser> GetUserRefreshToken(ApplicationUser user)
        {
            var findUserDetail = await _userManager.FindByNameAsync(user.UserName);
            if (findUserDetail == null) return null;
            return findUserDetail;
        }
        public async Task<bool> UpdateUserRefreshToken(ApplicationUser user)
        {
            var updateUser = await AddUserRefreshToken(user);
            if (updateUser == null) return false;
            return true;
        }

    }
}
