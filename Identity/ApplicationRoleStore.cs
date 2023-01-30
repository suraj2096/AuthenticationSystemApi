using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSystem.Identity
{
    public class ApplicationRoleStore:RoleStore<ApplicationRoles,ApplicationDbContext>
    {
        public ApplicationRoleStore(ApplicationDbContext context, IdentityErrorDescriber describer):base(context,describer)
        {

        }
    }
}
