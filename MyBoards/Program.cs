
using Microsoft.EntityFrameworkCore;
using MyBoards.Entities;

namespace MyBoards
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
                       
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
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
                    FirtsName = "User",
                    LastName = "One",
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
                    FirtsName = "User",
                    LastName = "Two",
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


            app.Run();
        }
    }
}
