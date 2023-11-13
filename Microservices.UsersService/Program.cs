using Microservices.Core;
using Microservices.UsersService.Context;
using Microservices.UsersService.Services.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microservices.UsersService
{
    // lab db
    // docker exec -t db pg_dumpall -c -U admin > dump.sql
    // docker exec db psql -h localhost -d postgres -U admin -c 'CREATE DATABASE laba'
    // cat dump.sql | docker exec -i db psql -h localhost -U admin -d laba

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var connectionDB = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<LabContext>(options =>
            {
                options.UseNpgsql(connectionDB);
            });

            builder.Services.AddControllers();

            var authOptions = builder.Configuration.GetSection("Auth");
            builder.Services.Configure<AuthOptions>(authOptions);

            builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();

            builder.Services.AddCors(options => options.AddDefaultPolicy(b =>
            {
                b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<LabContext>())
                {
                    context?.Database.SetCommandTimeout(60);
                    context?.Database.Migrate();
                }
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();
            

            app.MapControllers();

            app.Run();
        }
    }
}