using Microservices.UsersService.Models;
using Microservices.UsersService.Repositories;

namespace Microservices.UsersService.Services
{
    public class UserService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;

        public UserService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<IReadOnlyList<User>> GetAllUsers()
        {
            return await _usersRepository.GetAllUsers();
        }

        public async Task<User> AddUser(User user)
        {
            return await _usersRepository.AddUser(user);
        }

        public async Task<User?> GetUser(string email, string password)
        {
            return await _usersRepository.GetUser(email, password);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _usersRepository.GetUserByEmail(email);
        }
    }
}
