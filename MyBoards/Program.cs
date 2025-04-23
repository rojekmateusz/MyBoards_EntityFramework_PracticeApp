
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyBoards.Entities;
using System.Text.Json.Serialization;

namespace MyBoards;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        builder.Services.AddDbContext<MyBoardsContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("MyBoardsDb")));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<MyBoardsContext>();
        var pendingMigrations = dbContext.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            dbContext.Database.Migrate();
        }

        var users = dbContext.Set<User>().ToList();

        if (!users.Any())
        {
            var user1 = new User()
            {
                Email = "user1@test.com",
                FullName = "User One",
                Address = new Address()
                {
                    Country = "Polska",
                    City = "Warszawa",
                    Street = "Szeroka",
                    PostalCode = "00-001"
                }
            };

            var user2 = new User()
            {
                Email = "user2@test.com",
                FullName = "User Two",
                Address = new Address()
                {
                    Country = "Polska",
                    City = "Kraków",
                    Street = "D³uga",
                    PostalCode = "00-002"
                }
            };

            dbContext.Set<User>().AddRange(user1, user1);

            dbContext.SaveChanges();
        }

        app.MapGet("Epics_OnHold", async (MyBoardsContext dbContext) =>
        {
            var OnHoldEpics = await dbContext.Epics
                .Where(e => e.StateId == 4)
                .OrderBy(e => e.Priority)
                .ToListAsync();

            return OnHoldEpics;
        });

        app.MapGet("Users_TopComments", async (MyBoardsContext dbContext) =>
        {
            var TopCommnets = await dbContext.Comments
                .GroupBy(c => c.AuthorId)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToListAsync();

            var topAuthor = TopCommnets.First(u => u.Count == TopCommnets.Max(tc => tc.Count));

            var userDetails = dbContext.Users.First(f => f.Id == topAuthor.Key);

            return new
            {
                userDetails,
                commentCount = topAuthor.Count
            };
        });

        app.MapPost("Update_Epic", async (MyBoardsContext dbContext) =>
        {
            var epic = await dbContext.Epics.FirstAsync(f => f.Id == 1);

            epic.Area = "Updated area";
            epic.Priority = 1;
            epic.StartDate = DateTime.Now;

            await dbContext.SaveChangesAsync();

            return epic;
        }
        );

        app.MapPost("Create_Tag", async (MyBoardsContext dbContext) =>
        {
            Tag tag = new Tag()
            {
                Value = "New Tag"
            };

            await dbContext.Tags.AddAsync(tag);
            await dbContext.SaveChangesAsync();

            return tag;
        }
        );

        app.MapPost("Create_Address", async (MyBoardsContext dbContext) =>
        {
            var address = new Address()
            {
                Id = Guid.NewGuid(),
                Country = "Polska",
                City = "Gdañsk",
                Street = "D³uga",
                PostalCode = "00-001"
            };

            var user = new User()
            {
                Email = "user@test.com",
                FullName = "User Test",
                Address = address
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        );

        app.MapGet("Get_User", async (MyBoardsContext dbContext) =>
        {
            var user = await dbContext.Users
                .Include(u => u.Comments)
                .ThenInclude(c => c.WorkItem)
                .Include(u => u.Address)
                .FirstAsync(u => u.Id == Guid.Parse("68366dbe-0809-490f-cc1d-08da10ab0e61"));

            return user;
        });

        app.MapDelete("Delete_WorkItem", async (MyBoardsContext dbContext) =>
        {
            var workItemTags = await dbContext.WorkItemTag.Where(t => t.WorkItemId == 12).ToListAsync();
            dbContext.RemoveRange(workItemTags);

            var workItemTag = await dbContext.WorkItemTag.FirstAsync(wi => wi.WorkItemId == 16);
            dbContext.WorkItemTag.RemoveRange(workItemTag);

            await dbContext.SaveChangesAsync();
        });
                    
        app.Run();
    }
}
