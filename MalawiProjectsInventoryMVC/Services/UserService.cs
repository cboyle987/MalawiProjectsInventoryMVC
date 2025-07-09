using MalawiProjectsInventoryMVC.Constants;

namespace MalawiProjectsInventoryMVC.Services;

public interface IUserService
{
    public bool IsAdmin();
}

public class UserService(IHttpContextAccessor httpContextAccessor) : IUserService
{
    public bool IsAdmin()
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity.IsAuthenticated) return false;
        var roleType = user.Claims.FirstOrDefault(x => x.Type == "roleType");
        if (roleType == null) return false;
        return roleType.Value == RoleConstants.Admin;
    }
}