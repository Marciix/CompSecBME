using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;
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
            try
            {
                return await _context.Users.FirstAsync(u => u.Id == id);
            }
            catch (InvalidOperationException)
            {
                throw new UserNotExistsException();
            }
        }

        public async Task<User> GetUserByUserNameOrEmail(string userNameOrEmail)
        {
            try
            {
                return await _context.Users.FirstAsync(u =>
                    u.UserName == userNameOrEmail || u.Email == userNameOrEmail);
            }
            catch (InvalidOperationException)
            {
                throw new UserNotExistsException();
            }
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

            if (password.Length < 8)
                throw new InvalidUserDataException("Password must be at least 8 characters long");

            await ValidateUserData(user);

            UserHelper.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUser(long userId, string firstName, string lastName, long authUserId)
        {
            var user = await GetUserById(userId);
            // Check if user is able to modify a user (must be admin) or delete itself

            if (await IsUserNotAllowedToModifyUser(user, authUserId))
                throw new UserNotAllowedToModifyUserException();

            ValidateFirstName(firstName);
            ValidateLastName(lastName);

            user.FirstName = firstName;
            user.LastName = lastName;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUser(long userIdToDelete, long authUserId)
        {
            var user = await GetUserById(userIdToDelete);

            // Check if user is able to delete a user (must be admin) or delete itself
            if (await IsUserNotAllowedToModifyUser(user, authUserId))
                throw new UserNotAllowedToDeleteUserException();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserAdmin(long userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user != null && user.IsAdmin;
        }

        public async Task<bool> IsUserExistsByUserNameOrMail(string userNameOrEmail)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userNameOrEmail || u.Email == userNameOrEmail);
        }

        // Function that negates CheckIfUserAllowedToModifyUser to avoid ! in if conditions
        private async Task<bool> IsUserNotAllowedToModifyUser(User userToModify, long userId)
        {
            return !await CheckIfUserAllowedToModifyUser(userToModify, userId);
        }

        // Returns true if the authenticated user is admin or user wants modify itself
        private async Task<bool> CheckIfUserAllowedToModifyUser(User userToModify, long userId)
        {
            var isAdmin = await IsUserAdmin(userId);
            return isAdmin || userToModify.Id == userId;
        }

        private static void ValidateFirstName(string firstName)
        {
            const string pattern = @"^[a-zA-Z aáeéiíoóöőuúüűAÁEÉIÍOÓÖŐUÚÜŰ]{0,32}$";
            if (firstName == null || !Regex.IsMatch(firstName, pattern))
                throw new InvalidUserDataException(
                    "Last name must be maximum 32 chars long without any special character.");
        }

        private static void ValidateLastName(string lastName)
        {
            const string pattern = @"^[a-zA-Z .aáeéiíoóöőuúüűAÁEÉIÍOÓÖŐUÚÜŰ]{0,32}$";
            if (lastName == null || !Regex.IsMatch(lastName, pattern))
                throw new InvalidUserDataException(
                    "First name must be maximum 32 chars long without any special character.");
        }

        private async Task ValidateUserData(User user)
        {
            if (!Regex.IsMatch(user.UserName, @"^[a-zA-Z0-9_]{3,32}$"))
                throw new InvalidUserDataException(
                    "Username must be between 3 and 32 alphanumerical chars or underscore");

            ValidateFirstName(user.FirstName);

            ValidateLastName(user.LastName);

            try
            {
                var addr = new MailAddress(user.Email);
                if (addr.Address != user.Email)
                    throw new InvalidUserDataException("");
            }
            catch (Exception)
            {
                throw new InvalidUserDataException("Invalid Email address");
            }

            if (await _context.Users.AnyAsync(x => x.UserName == user.UserName))
                throw new UserAlreadyExistsException("Username \"" + user.UserName + "\" is already taken");

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                throw new UserAlreadyExistsException("Email \"" + user.Email + "\" is already taken");
        }
    }
}