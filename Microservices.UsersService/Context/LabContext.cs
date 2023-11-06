using Microservices.UsersService.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservices.UsersService.Context
{
    public class LabContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public LabContext(DbContextOptions<LabContext> options)
            : base(options)
        {
            // Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
