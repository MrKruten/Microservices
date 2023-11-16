using Microservices.UsersService.Models;

namespace Microservices.UsersService.Repositories
{
    public interface IUsersRepository
    {
        Task<User> AddUser(User user);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUser(string email, string password);
        Task<IReadOnlyList<User>> GetAllUsers();
    }
}
