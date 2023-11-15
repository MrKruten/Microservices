using Microservices.Core;
using Microservices.UsersService.Context;
using Microservices.UsersService.Services.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace Microservices.UsersService
{
    public class Startup
    {
        private IWebHostEnvironment _env;
        private IConfiguration _configuration;

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            var connectionDB = _configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<LabContext>(options =>
            {
                options.UseNpgsql(connectionDB);
            });

            services.AddControllers();

            var authOptions = _configuration.GetSection("Auth");
            services.Configure<AuthOptions>(authOptions);

            services.AddScoped<IRabbitMqService, RabbitMqService>();

            services.AddCors(options => options.AddDefaultPolicy(b =>
            {
                b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (_env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();

            app.UseEndpoints(e => e.MapControllers());
        }
    }
}
