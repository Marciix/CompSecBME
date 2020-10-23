using System.Collections.Generic;
using System.Threading.Tasks;
using CaffShop.Entities;
using CaffShop.Helpers;
using CaffShop.Interfaces;
using CaffShop.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CaffShop.Services
{
    public class UserService : IUserService
    {
        private readonly CaffShopContext _context;

        public UserService(CaffShopContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserById(long id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByUserNameOrEmail(string userNameOrEmail)
        {
            return await _context.Users.FirstOrDefaultAsync(u =>
                u.UserName == userNameOrEmail || u.Email == userNameOrEmail);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> CreateUser(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new PasswordRequiredException("Password is required");

            if (await _context.Users.AnyAsync(x => x.UserName == user.UserName))
                throw new UserAlreadyExistsException("Username \"" + user.UserName + "\" is already taken");
            
            if(await _context.Users.AnyAsync(u => u.Email == user.Email))
                throw new UserAlreadyExistsException("Email \"" + user.Email + "\" is already taken");

            UserHelper.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUser(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}