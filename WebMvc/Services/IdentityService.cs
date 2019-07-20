using WebMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Security.Claims;

namespace WebMvc.Services
{
    public class IdentityService : IIdentityService<ApplicationUser>
    {
        public ApplicationUser Get(IPrincipal principal)
        {
            if (principal is ClaimsPrincipal claims)
            {
                var user = new ApplicationUser()
                {
                    Email = claims.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value ?? "",
                    Id = claims.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? "",
                };

                return user;

            }

            throw new ArgumentException(message: "The principal must be a ClaimsPrincipal", paramName: nameof(principal));
        }
    }
}
