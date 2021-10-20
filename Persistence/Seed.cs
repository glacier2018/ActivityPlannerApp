using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var users = new List<AppUser>
                {
                    new AppUser{DisplayName = "Bob", UserName = "bob", Email = "bob@test.com"},
                    new AppUser{DisplayName = "Tom", UserName = "tom", Email = "tom@test.com"},
                    new AppUser{DisplayName = "Jane", UserName = "jane", Email = "jane@test.com"}
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                }

            }
            if (await context.Activities.AnyAsync()) return;
            var activitiesData = await System.IO.File.ReadAllTextAsync("../Persistence/ActivitiesData.json");
            var activities = JsonSerializer.Deserialize<List<Activity>>(activitiesData);

            await context.Activities.AddRangeAsync(activities);
            await context.SaveChangesAsync();

        }
    }
}