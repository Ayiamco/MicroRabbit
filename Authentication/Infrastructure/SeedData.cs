using Authentication.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Authentication.Infrastructure
{
    public class SeedData
    {
        public async static void EnsurePopulated(IApplicationBuilder app, IConfiguration config)
        {
            //verify that there are no pending migrations
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();

            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!context.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(RoleNames.Admin));
                await roleManager.CreateAsync(new IdentityRole(RoleNames.Employee));
                await roleManager.CreateAsync(new IdentityRole(RoleNames.Owner));
            }
        }
    }
}
