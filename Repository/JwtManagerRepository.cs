using AuthenticationSystem.Identity;
using AuthenticationSystem.Repository.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationSystem.Repository
{
    public class JwtManagerRepository : IJwtManagerRepository
    {
        private readonly AppSettingJWT _appSettingJWT;
        public JwtManagerRepository(IOptions<AppSettingJWT> appSettingJWT)
        {
            _appSettingJWT = appSettingJWT.Value;
        }

        public ApplicationUser GenerateRefreshToken(ApplicationUser user)
        {
            return GenerateJWTToken(user);
        }

        public ApplicationUser GenerateToken(ApplicationUser user)
        {
            return GenerateJWTToken(user);
        }
        public ApplicationUser GenerateJWTToken(ApplicationUser user)
        {
            var TokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettingJWT.SecretKey);
            var TokenDescritor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = TokenHandler.CreateToken(TokenDescritor);
            user.Token = TokenHandler.WriteToken(token);
            return user;
        }

        public ClaimsPrincipal GetClaimsFromExpiredToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}
