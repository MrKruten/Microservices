using Microservices.Core;

namespace Microservices.UsersService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            var authOptions = builder.Configuration.GetSection("Auth");
            builder.Services.Configure<AuthOptions>(authOptions);

            builder.Services.AddCors(options => options.AddDefaultPolicy(b =>
            {
                b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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