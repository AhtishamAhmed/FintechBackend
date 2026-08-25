using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistance.Context;

namespace Persistance.Seed
{
    /// Development-only seed data: fake accounts for manual testing via Swagger.
    /// Never used outside app.Environment.IsDevelopment().
    public static class DevDataSeeder
    {
        private const string DevPassword = "DevPass123!";

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            var customer1 = await EnsureUserAsync(userManager, "customer1@fintech.dev", "Ahmed", "Hashmi", "Customer");
            var customer2 = await EnsureUserAsync(userManager, "customer2@fintech.dev", "Ali", "Khan", "Customer");
            await EnsureUserAsync(userManager, "admin@fintech.dev", "System", "Admin", "Admin");
            await EnsureUserAsync(userManager, "support@fintech.dev", "Sara", "Agent", "SupportAgent");
            await EnsureUserAsync(userManager, "compliance@fintech.dev", "Faisal", "Officer", "ComplianceOfficer");

            await EnsureWalletAsync(context, customer1.Id, 50000m);
            await EnsureWalletAsync(context, customer2.Id, 10000m);

            await context.SaveChangesAsync();
        }

        private static async Task<ApplicationUser> EnsureUserAsync(
            UserManager<ApplicationUser> userManager, string email, string firstName, string lastName, string role)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                return existing;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = "0300" + Random.Shared.Next(1000000, 9999999)
            };

            await userManager.CreateAsync(user, DevPassword);
            await userManager.AddToRoleAsync(user, role);

            return user;
        }

        private static async Task EnsureWalletAsync(ApplicationDbContext context, string userId, decimal openingBalance)
        {
            var existing = await context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (existing != null)
            {
                return;
            }

            context.Wallets.Add(new Wallet
            {
                UserId = userId,
                Currency = "PKR",
                Balance = openingBalance
            });
        }
    }
}
