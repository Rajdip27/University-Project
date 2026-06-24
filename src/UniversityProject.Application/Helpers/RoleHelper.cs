using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UniversityProject.Application.Security;

namespace UniversityProject.Application.Helpers;

public static class RoleHelper
{
    public static bool CanManage(ClaimsPrincipal user) =>
   user.IsInRole(AppRoles.Admin) || user.IsInRole(AppRoles.Editor);

    public static bool CanDelete(ClaimsPrincipal user) =>
        user.IsInRole(AppRoles.Admin);
}


