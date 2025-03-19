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
using Trackr.Infrastructure.DTO;
using AutoMapper;
using System.Security.Claims;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Trackr.Domain.Models.Database;

namespace Trackr.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UserRepository(AppDbContext context, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Result<bool>> RegisterUserAsync(User user)
        {
            if (user.PasswordHash == null)
                return Result<bool>.Failure("NullPassword", "Password is required.");
            var result = await _userManager.CreateAsync(user, user.PasswordHash);

            return _mapper.Map<Result<bool>>(result);

        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<Result<bool>> VerifyPasswordAsync(User userCredentials, string password)
        {
            if(userCredentials.PasswordHash == null || userCredentials.Email == null)
            {
                return Result<bool>.Failure("NullPasswordOrEmail", "Password and email are required.");
            }
            var result = await _userManager.CheckPasswordAsync(userCredentials, password);
            if (!result)
            {
                return Result<bool>.Failure("Invalid credentials", "Invalid email or password.");
            }
            else return Result<bool>.Success(result);
        }

        public async Task<Result<string>> GetRefreshTokenAsync( ClaimsPrincipal id, AuthProvider providerName)
        {
            User? user = await _userManager.GetUserAsync(id) ??
                throw new InvalidOperationException("Failed to retrieve user from database.");

            string loginProvider = providerName.ToString();
            string? result = await _userManager.GetAuthenticationTokenAsync(user, loginProvider, "refresh_token");

            return result is not null
                ? Result<string>.Success(result)
                : Result<string>.Failure("TokenNull", "Refresh token wasn't found.");
        }


        public async Task<Result<bool>> SetRefreshTokenAsync(ClaimsPrincipal id, string refreshToken, AuthProvider providerName)
        {
            if(!id.Identity!.IsAuthenticated) return Result<bool>.Failure("401", "User is not authenticated.");

            if (refreshToken == null) return Result<bool>.Failure("TokenNull", "Refresh token wasn't specified.");

            User? user = await _userManager.GetUserAsync(id) ??
                throw new InvalidOperationException("Failed to retrieve user from database.");

            IdentityResult result = await _userManager.SetAuthenticationTokenAsync(user, providerName.ToString(), "refresh_token", refreshToken);

            return _mapper.Map<Result<bool>>(result);
        }

        
    }
}
