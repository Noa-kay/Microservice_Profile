using System.Security.Claims;

namespace student_profile.Common.Security;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var rawUserId =
            user.FindFirstValue(ClaimTypes.NameIdentifier) ??
            user.FindFirstValue("sub") ??
            user.FindFirstValue("userId");

        return Guid.TryParse(rawUserId, out var userId) ? userId : null;
    }

    public static bool IsInAnyRole(this ClaimsPrincipal user, params string[] roles)
    {
        foreach (var role in roles)
        {
            if (user.IsInRole(role))
            {
                return true;
            }
        }

        return false;
    }
}
