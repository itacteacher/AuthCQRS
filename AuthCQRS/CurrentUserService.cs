using AuthCQRS.Application.Common.Interfaces;
using System.Security.Claims;

namespace AuthCQRS.Web;

public class CurrentUserService : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService (IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
