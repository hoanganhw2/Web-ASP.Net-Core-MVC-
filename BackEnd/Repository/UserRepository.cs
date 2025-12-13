using BackEnd.Data;
using BackEnd.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repository
{
    public class UserRepository : IUserRepository
    {

        private readonly BookDbContext _bookDbContext;

        public UserRepository(BookDbContext bookDbContext)
        {
            _bookDbContext = bookDbContext;
        }

        public async Task<User?> addUser(User user)
        {          
            _bookDbContext.Users.Add(user);
            await _bookDbContext.SaveChangesAsync();       
            return user;
        }

        public async Task deleteUser(int id)
        {
            var user = await _bookDbContext.Users.FindAsync(id);
            if(user !=null)
            {
                _bookDbContext.Users.Remove(user);
                await _bookDbContext.SaveChangesAsync();
            } 
        }

        public async Task<IEnumerable<User>> getAllUser()
        {
            return await  _bookDbContext.Users.Include(u=>u.role).ToListAsync();
        }

        public async Task<User?> getUserByEmail(string email)
        {
            return await _bookDbContext.Users.FirstOrDefaultAsync(u => u.email == email);
        }

        public async Task<User?> getUserById(int id)
        {
            return await _bookDbContext.Users.Include(u => u.role).FirstOrDefaultAsync(u=>u.id==id);
        }

        public async Task<User?> getUserByUsername(string username)
        {
           return await _bookDbContext.Users.FirstOrDefaultAsync(u=>u.username==username);
        }

        public async Task updateUser(User user)
        {
            _bookDbContext.Users.Update(user);
            await _bookDbContext.SaveChangesAsync();
        }

        
    }
}
