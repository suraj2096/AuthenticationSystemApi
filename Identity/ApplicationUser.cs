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
        
        public string? RefreshToken { get; set; }
        [NotMapped]
        public string? Role { get; set; }
        

    }
}
