using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSystem.Identity
{
    public class ApplicationRoleManager:RoleManager<ApplicationRoles>
    {
        public ApplicationRoleManager(ApplicationRoleStore appRoleStore,
    IEnumerable<IRoleValidator<ApplicationRoles>> roleValidators,
    ILookupNormalizer lookupNormalizer, IdentityErrorDescriber identityErrorDescriber,
    ILogger<ApplicationRoleManager> logger) : base(appRoleStore, roleValidators, lookupNormalizer,
      identityErrorDescriber, logger)
        {

        }

    }
}
