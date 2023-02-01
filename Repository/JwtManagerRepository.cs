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
using System.Security.Cryptography;
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
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettingJWT.SecretKey);
            var tokenDescritor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescritor);
            user.Token = tokenHandler.WriteToken(token);
            user.RefreshToken = GenerateRefreshToken();
            return user;
        }
        public string GenerateRefreshToken()
        {
            var randomeNumber = new byte[32];
            using (var rNG = RandomNumberGenerator.Create())
            {
                rNG.GetBytes(randomeNumber);
                return Convert.ToBase64String(randomeNumber);
            }

        }

        public ClaimsPrincipal GetClaimsFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettingJWT.SecretKey);
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };
            try
            {
            var claimUserValue = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
            return claimUserValue;
            }
            catch
            {
                return null;
            }
        }
    }
}
