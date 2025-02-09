using AutoMapper;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Interfaces;
using Trackr.Domain.Models;
using Trackr.Infrastructure.DTO;
using Trackr.Infrastructure.Extensions;
using Trackr.Infrastructure.Interfaces;

namespace Trackr.Infrastructure
{
    public class SpotifyClient : IClient
    {
        public AuthProvider ProviderName { get => AuthProvider.Spotify; }

        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SpotifyClient(HttpClient httpClient, IMapper mapper, IConfiguration configuration) 
        {  
            _httpClient = httpClient;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Result<Tokens>> RequestTokensAsync(string code)
        {
            var requestBody = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code" },
                {"code", code},
                {"redirect_uri",  _configuration["SpotifyClient:RedirectUri"]!}
            };
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            requestMessage.Content = new FormUrlEncodedContent(requestBody);
            var encodedIds = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration["SpotifyClient:ClientId"]}:{_configuration["SpotifyClient:ClientSecret"]}"));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedIds);

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return Result<Tokens>.Failure("TokensRequestError", await response.Content.ReadAsStringAsync());

            var responseBody = await response.Content.ReadAsStringAsync();
            SpotifyTokensDTO tokens = JsonConvert.DeserializeObject<SpotifyTokensDTO>(responseBody)!;

            Tokens result = _mapper.Map<Tokens>(tokens);

            return Result<Tokens>.Success(result);
        }

        public async Task<Result<Tokens>> RefreshAccessTokenAsync(string refreshToken)
        {
            var requestBody = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token" },
                {"refresh_token", refreshToken}
            };
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            requestMessage.Content = new FormUrlEncodedContent(requestBody);
            var encodedIds = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration["SpotifyClient:ClientId"]}:{_configuration["SpotifyClient:ClientSecret"]}"));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedIds);

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return await response.HandleError<Tokens>();


            var responseBody = await response.Content.ReadAsStringAsync();
            SpotifyTokensDTO tokens = JsonConvert.DeserializeObject<SpotifyTokensDTO>(responseBody)!;
            tokens.refresh_token ??= refreshToken;
            Tokens result = _mapper.Map<Tokens>(tokens);

            return Result<Tokens>.Success(result);
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

        public async Task<Result<Tracks>> GetTracksAfterTime(string authToken, long after)
        {
            if (authToken == null || after < 0 || after > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) return Result<Tracks>.Failure("NullParameters", "Incorrect parameters.");

            Dictionary<string, string> queryParams = new Dictionary<string, string> {
                { "limit", "50" },
                { "after",after.ToString() }
            };
            string fullUrl = QueryHelpers.AddQueryString("https://api.spotify.com/v1/me/player/recently-played", queryParams);


            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            

            HttpResponseMessage? response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return await response.HandleError<Tracks>();


            string? responseBody = await response.Content.ReadAsStringAsync();
            SpotifyTracksDTO? spotifyTracks = JsonConvert.DeserializeObject<SpotifyTracksDTO>(responseBody);

            Tracks tracks = _mapper.Map<Tracks>(spotifyTracks);

            return Result<Tracks>.Success(tracks);

        }

        
    }
}
