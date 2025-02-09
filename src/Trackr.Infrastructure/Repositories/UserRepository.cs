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

        /*public async Task<IdentityResult> RegisterUserAsync1(User userInfo)
        {
            var existingUserByEmail = await _userManager.FindByEmailAsync(userInfo.Email);
            if (existingUserByEmail != null) return IdentityResult.Failed(new IdentityError{Code = "DuplicateEmail", Description = "User with such email already exists." });

            var result = await _userManager.CreateAsync(userInfo, userInfo.PasswordHash);
            *//*List<string> errors = new();
            foreach (var error in result.Errors)
            {
                errors.Add(error.Description);
            }
            if (!result.Succeeded) return Result.Failure(errors);

            return Result.Success();*//*

            return result;
        }*/

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
            //Console.WriteLine("Password: "+userCredentials.PasswordHash);
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

        public async Task<Result<long>> GetLastPlayedTrackTimeFromDb(string id)
        {
            if (string.IsNullOrEmpty(id)) return Result<long>.Failure("401", "User is not authenticated.");

            DateTime time = await _context.Listened.Where(listen => listen.UserId == id).Select(l=>l.ListenedAt).FirstOrDefaultAsync();
            long after = 0;
            if(time != DateTime.MinValue) after = ((DateTimeOffset)time).ToUnixTimeMilliseconds();

            return Result<long>.Success(after);
        }

        public async Task SaveReceivedTracksToDbAsync(string id, Tracks tracks)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (tracks!=null && tracks.TracksArray != null)
            {
                TrackItem[] items = tracks.TracksArray;
                //add track 
                //add album
                //add listened tracks
            }
            
        }
    }
}
