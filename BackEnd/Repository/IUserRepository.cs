using BackEnd.Models.Entity;

namespace BackEnd.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> getAllUser();
        Task<User?> getUserById(int id);
        Task<User?> getUserByEmail(string email);
        Task<User?> getUserByUsername(string username);
        Task<User?> addUser(User user);
        Task updateUser(User user);
        Task deleteUser(int id);
       
        
    }
}
