using Microservices.UsersService.Context;
using Microservices.UsersService.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservices.UsersService.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly LabContext _db;

        public UsersRepository(LabContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<User>> GetAllUsers()
        {
            var users = await _db.Users.ToListAsync();
            return users;
        }

        public async Task<User> AddUser(User user)
        {
            var result = await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<User?> GetUser(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            return user;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }
    }
}
