using AuthenticationSystem.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSystem.Repository.IRepository
{
    public interface IUserServiceRepository
    {
         Task<bool> IsUnique(string userName);
         Task<ApplicationUser> AuthenticateUser(string userName, string userPassword);
         Task<bool> RegisterUser(ApplicationUser userCredentials);
    }
}
