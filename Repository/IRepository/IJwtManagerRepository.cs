using AuthenticationSystem.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationSystem.Repository.IRepository
{
    public interface IJwtManagerRepository
    {
       ApplicationUser GenerateToken(ApplicationUser user,bool isGenerateRefreshToken);
        ClaimsPrincipal GetClaimsFromExpiredToken(string token);
    }
}
