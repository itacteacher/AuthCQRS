using AuthCQRS.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AuthCQRS.Infrastructure.Identity;
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService (UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsFactory,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _userClaimsFactory = userClaimsFactory;
        _authorizationService = authorizationService;
    }

    public async Task<bool> AuthorizeAsync (string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<(string Result, string UserId)> CreateUserAsync (string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result?.ToString()!, user.Id);
    }

    public async Task<string> DeleteUserAsync (string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return IdentityResult.Success.ToString();
        }

        var result = await _userManager.DeleteAsync(user);

        return result.ToString();
    }

    public async Task<string?> GetUserNameAsync (string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<bool> IsInRoleAsync (string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        return await _userManager.IsInRoleAsync(user!, role);
    }
}
