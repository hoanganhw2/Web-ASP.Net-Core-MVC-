using BackEnd.Models.Entity;

namespace BackEnd.Service
{
    public interface IUserService
    {
        public IEnumerable<User> getAllUsers();
        public Task<Int32> addUser(User user);
        public bool update(User user);
        public bool removeUser(User user);
        public User? userLogin(string username, string password);
    }
}
