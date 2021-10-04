using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context)
        {
            if (await context.Activities.AnyAsync()) return;
            var activitiesData = await System.IO.File.ReadAllTextAsync("../Persistence/ActivitiesData.json");
            var activities = JsonSerializer.Deserialize<List<Activity>>(activitiesData);

            await context.Activities.AddRangeAsync(activities);
            await context.SaveChangesAsync();

        }
    }
}