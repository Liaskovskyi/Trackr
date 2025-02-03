using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Interfaces;
using Trackr.Domain.Models;
using Trackr.Infrastructure.DTO;
using Trackr.Infrastructure.Interfaces;

namespace Trackr.Infrastructure
{
    public class SpotifyClient : IClient
    {
        public AuthProvider ProviderName { get => AuthProvider.Spotify; }

        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public SpotifyClient(HttpClient httpClient, IMapper mapper) 
        {  
            _httpClient = httpClient;
            _mapper = mapper;
        }

        public async Task<PlaybackState> GetPlaybackStateAsync(string AuthToken)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/player");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var spotifyPlaybackState = JsonConvert.DeserializeObject<SpotifyPlaybackState>(responseBody);
            
            var playbackState = _mapper.Map<PlaybackState>(spotifyPlaybackState);

            return playbackState;
        }
    }
}
