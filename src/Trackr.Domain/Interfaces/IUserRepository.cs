using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;

namespace Trackr.Domain.Interfaces
{
    public interface IUserRepository
    {
        //public Task<string> GetAuthTokenAsync(int id, AuthProvider providerName);
        public Task<User?> FindByEmailAsync(string email);
        public Task<Result<bool>> RegisterUserAsync(User registerInfo); 
        public Task<Result<bool>> VerifyPasswordAsync(User userCredentials, string password);
        public Task<Result<string>> GetRefreshTokenAsync(ClaimsPrincipal id, AuthProvider providerName);
        public Task<Result<bool>> SetRefreshTokenAsync(ClaimsPrincipal id, string refreshToken, AuthProvider providerName);
        public Task<Result<long>> GetLastPlayedTrackTimeFromDb(string id);
        public Task SaveReceivedTracksToDbAsync(string id, Tracks tracks);
    }
}
