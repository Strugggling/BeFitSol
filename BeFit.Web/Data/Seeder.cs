 
using BeFit.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Web.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            await context.Database.EnsureCreatedAsync();
            
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            string adminRole = "Administrator";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            string adminEmail = "admin@befit.local";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, adminRole);
            }

            string testEmail = "test@befit.local";
            var testUser = await userManager.FindByEmailAsync(testEmail);
            if (testUser == null)
            {
                testUser = new AppUser { UserName = testEmail, Email = testEmail, EmailConfirmed = true };
                await userManager.CreateAsync(testUser, "Test123!");
            }

            string userEmail1 = "user@mail.pl";
            string userEmail2 = "admin@mail.pl";
            var user1 = await userManager.FindByEmailAsync(userEmail1);
            if (user1 == null)
            {
                user1 = new AppUser { UserName = userEmail1, Email = userEmail1, EmailConfirmed = true };
                await userManager.CreateAsync(user1, "Pas123!");
            }

            var user2 = await userManager.FindByEmailAsync(userEmail2);
            if (user2 == null)
            {
                user2 = new AppUser { UserName = userEmail2, Email = userEmail2, EmailConfirmed = true };
                await userManager.CreateAsync(user2, "Pas123!");
            }

            if (user2 != null && !await userManager.IsInRoleAsync(user2, adminRole))
            {
                await userManager.AddToRoleAsync(user2, adminRole);
            }

            var user1Id = user1?.Id;
            var user2Id = user2?.Id;

            if (!await context.ExerciseTypes.AnyAsync())
            {
                var types = new[] {
                    new ExerciseTyp { Name = "Squat" },
                    new ExerciseTyp { Name = "Bench Press" },
                    new ExerciseTyp { Name = "Deadlift" }
                };
                context.ExerciseTypes.AddRange(types);
                await context.SaveChangesAsync();
            }

            var firstType = await context.ExerciseTypes.OrderBy(t => t.Id).FirstAsync();

            if (user1Id != null && !await context.TrainingSessions.AnyAsync(ts => ts.UserId == user1Id))
            {
                var s = new Training
                {
                    StartAt = DateTime.UtcNow.AddDays(-2).AddHours(-1),
                    EndAt = DateTime.UtcNow.AddDays(-2),
                    UserId = user1Id
                };
                context.TrainingSessions.Add(s);
                await context.SaveChangesAsync();

                context.PerformedExercises.Add(new DoneExercise
                {
                    SessionId = s.Id,
                    ExerciseTypeId = firstType.Id,
                    UserId = user1Id,
                    Sets = 3,
                    Repetitions = 8,
                    Weight = 60m
                });
                await context.SaveChangesAsync();
            }

            if (user2Id != null && !await context.TrainingSessions.AnyAsync(ts => ts.UserId == user2Id))
            {
                var s1 = new Training
                {
                    StartAt = DateTime.UtcNow.AddDays(-1).AddHours(-2),
                    EndAt = DateTime.UtcNow.AddDays(-1).AddHours(-1),
                    UserId = user2Id
                };
                var s2 = new Training
                {
                    StartAt = DateTime.UtcNow.AddDays(-7).AddHours(-1),
                    EndAt = DateTime.UtcNow.AddDays(-7),
                    UserId = user2Id
                };
                context.TrainingSessions.AddRange(s1, s2);
                await context.SaveChangesAsync();

                context.PerformedExercises.Add(new DoneExercise
                {
                    SessionId = s1.Id,
                    ExerciseTypeId = firstType.Id,
                    UserId = user2Id,
                    Sets = 4,
                    Repetitions = 6,
                    Weight = 70m
                });

                context.PerformedExercises.Add(new DoneExercise
                {
                    SessionId = s2.Id,
                    ExerciseTypeId = firstType.Id,
                    UserId = user2Id,
                    Sets = 5,
                    Repetitions = 5,
                    Weight = 80m
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
