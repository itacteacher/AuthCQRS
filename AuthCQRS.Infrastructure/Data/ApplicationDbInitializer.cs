using AuthCQRS.Domain.Enums;
using AuthCQRS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCQRS.Infrastructure.Data;
public class ApplicationDbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbInitializer (ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync ()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task TrySeedAsync ()
    {
        var roles = Enum.GetNames(typeof(Roles));

        foreach (var role in roles)
        {
            if (_roleManager.Roles.All(r => r.Name != role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var admin = new ApplicationUser
        {
            UserName = "admin@test.com",
            Email = "admin@test.com"
        };

        if (_userManager.Users.All(u => u.UserName != admin.UserName))
        {
            await _userManager.CreateAsync(admin, "Admin123_");
            await _userManager.AddToRolesAsync(admin, [Roles.Administrator.ToString()]);
        }
    }
}

public static class InitializerExtensions
{
    public static async Task InitializeDbAsync (this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>();

        await initializer.SeedAsync();
    }
}
