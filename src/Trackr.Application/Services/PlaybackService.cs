using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Application.Interfaces;
using Trackr.Domain.Models;
using Trackr.Domain.Interfaces;
using System.Security.Claims;

namespace Trackr.Application.Services
{
    public class PlaybackService : IPlaybackService
    {
        private readonly IClient _client;
        private readonly IAuthService _authService;
        //private readonly IUserRepository _userRepository;

        public PlaybackService(IClient client, IAuthService authService) //, IUserRepository userRepositiry)
        {
            _client = client;
            _authService = authService;
            //_userRepository = userRepositiry;
        }

        public async Task<PlaybackState> GetCurrentTrackAsync(ClaimsPrincipal claimsPrincipal)
        {
            //string authToken =  await _userRepository.GetAuthTokenAsync(userId, _client.ProviderName) ?? throw new ArgumentException("No token found");
            //string authToken = "BQCxxy5AxITbWjHn3J_Mcy8XYF_p8HHd7pN4vvjfaSNf66mh84AgEhJt_XXYN5PEAVr5EU0uhqheywI9C9zanf8F7X3oL5XKlhP6JIwnWvA9N6T1W-kv2BEdnXez2PwQLeUDHgoAuC0wDCYEvtTgoCQIneVZipr8obZRCECuaOnn-sFjkDLnW5QLTHLe1oJADpW4-4cnhgSVjjdPXl438flkHFn40UlmB4RBxRhFhi2j6mfdUQ";
            string token = await _authService.GetCachedToken(claimsPrincipal);

            PlaybackState playBackState = await _client.GetPlaybackStateAsync(token);

            return playBackState;
        }
    }
}
