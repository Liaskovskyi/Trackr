using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Trackr.Application.Interfaces;
using Trackr.Domain.Interfaces;
using Trackr.Domain.Models;

namespace Trackr.Application.Services
{
    public class TrackService : ITrackService
    {
        private IClient _client;
        private IUserRepository _userRepository;
        private IAuthService _authService;

        public TrackService(IUserRepository userRepository, IAuthService authService, IClient client)
        {
            _userRepository = userRepository;
            _authService = authService;
            _client = client;
        }

        public async Task<Result<Tracks>> GetLastPlayedTracks(ClaimsPrincipal claimsPrincipal)
        {
            string? userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("UserId is missing.");

            Result<long> time = await GetLastPlayedTrackTime(claimsPrincipal);
            long after = time.Value;

            string token = await _authService.GetCachedToken(claimsPrincipal);

            Result<Tracks> tracks = await _client.GetTracksAfterTime(token, after);

            await _userRepository.SaveReceivedTracksToDbAsync(userId, tracks.Value);

            return tracks;
        }

        public async Task<Result<long>> GetLastPlayedTrackTime(ClaimsPrincipal claimsPrincipal)
        {
            string? userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("UserId is missing.");

            Result<long> after = await _userRepository.GetLastPlayedTrackTimeFromDb(userId);
            if(!after.IsSuccess) return Result<long>.Failure(after.Errors);

            return after;
        }
    }
}
