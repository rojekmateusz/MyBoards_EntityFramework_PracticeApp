
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
            
            app.Run();
        }
    }
}
