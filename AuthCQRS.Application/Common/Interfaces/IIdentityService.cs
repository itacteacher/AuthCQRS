namespace AuthCQRS.Application.Common.Interfaces;
public interface IIdentityService
{
    Task<(string Result, string UserId)> CreateUserAsync (string userName, string password);
    Task<string> DeleteUserAsync (string userId);
    Task<bool> AuthorizeAsync (string userId, string policyName);
    Task<string?> GetUserNameAsync (string userId);
    Task<bool> IsInRoleAsync (string userId, string role);
}
