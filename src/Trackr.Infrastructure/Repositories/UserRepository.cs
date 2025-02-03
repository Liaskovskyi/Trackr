using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Interfaces;
using Trackr.Domain.Models;
using Trackr.Infrastructure.DataBase;
using Microsoft.AspNetCore.Identity;

namespace Trackr.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> RegisterUserAsync(User userInfo)
        {
            var existingUserByEmail = await _userManager.FindByEmailAsync(userInfo.Email);
            if (existingUserByEmail != null) throw new AuthException("User with such email already exists.");

            var result = await _userManager.CreateAsync(userInfo, userInfo.PasswordHash);

            return result.Succeeded;
        }

    }
}
