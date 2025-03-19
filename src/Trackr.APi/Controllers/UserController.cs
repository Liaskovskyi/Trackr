using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Trackr.Api.Extensions;
using Trackr.Api.Models;
using Trackr.Application.Interfaces;
using Trackr.Domain.Models;
using Trackr.Domain.Models.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Trackr.Api.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ITrackService _trackService;

        public UserController(IAuthService authService, IMapper mapper, IConfiguration configuration, ITrackService trackService)
        {
            _authService = authService;
            _mapper = mapper;
            _configuration = configuration;
            _trackService = trackService;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDTO userInfo)
        {
            var user = _mapper.Map<User>(userInfo);
            Result<bool> result = await _authService.RegisterAsync(user);         

            if (!result.IsSuccess)
            {
                ModelStateDictionary state = new ModelStateDictionary();
                foreach (var error in result.Errors) state.AddModelError(error.Code, error.Description);
                var problemDetails = new ValidationProblemDetails(state);
                return BadRequest(problemDetails);
            }

            Result<Tracks> getHistory = await _trackService.UpdateLastPlayedTracks(User);
            if (!getHistory.IsSuccess) return BadRequest(getHistory);

            return Ok($"User {userInfo.Username} registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO request)
        {
            var user = _mapper.Map<User>(request);
            Result<Tokens> tokens = await _authService.LoginAsync(user);

            if (!tokens.IsSuccess)
            {
                ModelStateDictionary state = new ModelStateDictionary();
                foreach (var error in tokens.Errors) state.AddModelError(error.Code, error.Description);
                var problemDetails = new ValidationProblemDetails(state);
                return BadRequest(problemDetails);
            }

            JWTDTO response = new JWTDTO(tokens.Value.AccessToken, tokens.Value.RefreshToken);

            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshAccessToken()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            Result<string> token = await _authService.RefreshGWT(userId);
            if (!token.IsSuccess) return BadRequest(token.RetrieveErrors());

            AccessJWT newToken = new AccessJWT(token.Value!);

            return Ok(newToken);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("spotify/login")]
        public  IActionResult SpotifyLogin()
        {
            string clientId = _configuration["SpotifyClient:ClientId"]!;
            string scope = "user-read-playback-state user-read-recently-played";
            string redirectUri = _configuration["SpotifyClient:RedirectUri"]!;
            string state = _configuration["SpotifyClient:State"]!;
            string uri = "https://accounts.spotify.com/authorize?" +
                "response_type=code" +
                $"&client_id={clientId}" +
                $"&scope={scope}" +
                $"&redirect_uri={redirectUri}" +
                $"&state={state}";

            return Ok(uri);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("callback")]
        public async Task<IActionResult> SpotifyCallback(string code, string state)
        {
            if (state != _configuration["SpotifyClient:State"]) return BadRequest("Wrong state, request denied");

            Result<bool> result = await _authService.GetClientTokens(User, code);

            if(!result.IsSuccess) return BadRequest(result.RetrieveErrors());

            return Ok("Spotify connected successfully.");

        }

        //[Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("spotify-refresh")]
        public async Task<IActionResult> SpotifyRefreshToken(string code)
        {
            Result<Tokens> tokens = await _authService.RefreshClientToken(code);

            if (!tokens.IsSuccess) return BadRequest(tokens.RetrieveErrors());

            JWTDTO response = new JWTDTO(tokens.Value.AccessToken, tokens.Value.RefreshToken);

            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("secure-data")]
        public IActionResult GetSecureData()
        {
            string? id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(new { Message = $"You are authenticated! Id: {id}" });
        }
    }
}
