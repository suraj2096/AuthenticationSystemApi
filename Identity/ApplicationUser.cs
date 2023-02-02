using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSystem.Identity
{
    public class ApplicationUser:IdentityUser
    {
        [NotMapped]
        public string Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenValidDate { get; set; }
        [NotMapped]
        public string? Role { get; set; }
        

    }
}
