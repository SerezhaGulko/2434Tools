using _2434Tools.Data;
using _2434ToolsUser.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2434Tools.Services
{
    public class UserPermissionsService : IUserPermissionsService
    {
        private readonly HttpContext _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserPermissionsService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            this._httpContext   = httpContextAccessor.HttpContext;
            this._userManager   = userManager;
        }

        public bool IsAdmin()
        {
            return this._httpContext.User.IsInRole(ApplicationRoles.Administrators);
        }
    }
}
