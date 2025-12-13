using BackEnd.Data;
using BackEnd.Models.Entity;
using BackEnd.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BackEnd.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly BookDbContext _bookDbContext;
        private IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository,BookDbContext bookDbContext,IPasswordHasher<User> passwordHasher)
        {
            this._userRepository = userRepository;
            this._bookDbContext = bookDbContext;
            this._passwordHasher = passwordHasher;
        }

        public async Task<Int32> addUser(User user)
        {
            var exitUser = await _userRepository.getUserByEmail(user.email);
            var exitUser2 =await _userRepository.getUserByUsername(user.username);
            if (exitUser != null) return -1;
            if (exitUser2 != null) return 0;
            var passwordHas = _passwordHasher.HashPassword(user, user.password);
            user.password = passwordHas;
            await _userRepository.addUser(user);
            return 1;
        }

        public IEnumerable<User> getAllUsers()
        {
            throw new NotImplementedException();
        }

        public bool removeUser(User user)
        {
            throw new NotImplementedException();
        }

        public bool update(User user)
        {
            throw new NotImplementedException();
        }

        public User? userLogin(string username, string password)
        {
            var user = _bookDbContext.Users
                .Include(u => u.role)
                .FirstOrDefault(user => user.username == username);
            if (user == null) return null;
            else
            {
                return _passwordHasher.VerifyHashedPassword(user, user.password, password) == PasswordVerificationResult.Success ? user : null;
            }
        }

       
    }
}
