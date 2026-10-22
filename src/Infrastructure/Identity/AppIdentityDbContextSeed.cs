using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.eShopWeb.Infrastructure.Identity;

public class AppIdentityDbContextSeed
{
    public const string DEMO_USER_PASSWORD_CONFIG_NAME = "DemoUserPassword";
    public const string PRODUCT_MANAGER_PASSWORD_CONFIG_NAME = "ProductManagerPassword";
    public const string ADMIN_PASSWORD_CONFIG_NAME = "AdminPassword";

    public static async Task SeedAsync(AppIdentityDbContext identityDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        string? demoUserPassword, string? productManagerPassword, string? adminPassword)
    {

        if (identityDbContext.Database.IsSqlServer())
        {
            identityDbContext.Database.Migrate();
        }

        await roleManager.CreateAsync(new IdentityRole(BlazorShared.Authorization.Constants.Roles.ADMINISTRATORS));
        await roleManager.CreateAsync(new IdentityRole(BlazorShared.Authorization.Constants.Roles.PRODUCT_MANAGERS));

        // Each demo user is only created when its password is configured
        await CreateUserAsync(userManager, "demouser@microsoft.com", demoUserPassword, null);
        await CreateUserAsync(userManager, "productmgr@microsoft.com", productManagerPassword, BlazorShared.Authorization.Constants.Roles.PRODUCT_MANAGERS);
        await CreateUserAsync(userManager, "admin@microsoft.com", adminPassword, BlazorShared.Authorization.Constants.Roles.ADMINISTRATORS);
    }

    private static async Task CreateUserAsync(UserManager<ApplicationUser> userManager, string userName, string? password, string? role)
    {
        if (string.IsNullOrEmpty(password)) return;

        await userManager.CreateAsync(new ApplicationUser { UserName = userName, Email = userName }, password);
        var user = await userManager.FindByNameAsync(userName);
        if (user != null && role != null)
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
