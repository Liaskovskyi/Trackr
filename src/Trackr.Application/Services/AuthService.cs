using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Trackr.Application.Interfaces;
using Trackr.Domain.Interfaces;
using Trackr.Domain.Models;
using Trackr.Domain.Models.Database;

namespace Trackr.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IJwtGenerator _jwtTokenGenerator;
        private readonly IClient _client;
        private readonly ICache _cache;

        public AuthService(IUserRepository userRepository, IMapper mapper, IJwtGenerator jwtTokenGenerator, IClient client, ICache cache)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _client = client;   
            _cache = cache;
        }

        public async Task<Result<bool>> RegisterAsync(User registerInfo)
        {
            if (registerInfo.Email == null) 
                return Result<bool>.Failure(new List<ResultError> { new ResultError("EmailNull", "Email is required.") });

            User? existingUser = await _userRepository.FindByEmailAsync(registerInfo.Email);

            if (existingUser != null)
            {
                return Result<bool>.Failure(new List<ResultError>{new ResultError("DuplicateEmail", "User with this email already exists.")});
            }

            User? user = _mapper.Map<User>(registerInfo);
            Result<bool> createResult = await _userRepository.RegisterUserAsync(user);

            return createResult;
        }

        public async Task<Result<Tokens>> LoginAsync(User registerInfo)
        {
            var validationResult = await ValidateCredentials(registerInfo);
            if (!validationResult.IsSuccess)
            {
                return Result<Tokens>.Failure(validationResult.Errors);
            }

            Tokens tokens = GenerateTokens(validationResult.Value);

            return Result<Tokens>.Success(tokens);
        }

        private async Task<Result<User>> ValidateCredentials(User registerInfo)
        {
            if (registerInfo.Email == null || registerInfo.PasswordHash == null)
                return Result<User>.Failure(new List<ResultError> { new ResultError("NullField", "Both email and password are required.") });

            User? existingUser = await _userRepository.FindByEmailAsync(registerInfo.Email);
            if (existingUser == null)
            {
                return Result<User>.Failure(new List<ResultError> { new ResultError("NoSuchEmail", "User with the specified email wasn't found") });
            }

            Result<bool> valid = await _userRepository.VerifyPasswordAsync(existingUser, registerInfo.PasswordHash);
            if (!valid.IsSuccess)
            {
                return Result<User>.Failure(new List<ResultError> { new ResultError("InvalidCredentials", "Invalid email or password.") });
            }

            return Result<User>.Success(existingUser);
        }

        public async Task<Result<string>> RefreshGWT(string userId)
        {
            if (userId == "") 
            {
                return Result<string>.Failure(new List<ResultError> { new ResultError("NoSuchUser", "User with such Id wasn't found.") });
            }
            string newToken = _jwtTokenGenerator.GenerateAccessJWT(userId);
            if(newToken == "") return Result<string>.Failure(new List<ResultError> { new ResultError("GWTError", "Couldn't generate new GWT.") });
            return Result<string>.Success(newToken);

        }

        private Tokens GenerateTokens(User user)
        {
            return new Tokens(_jwtTokenGenerator.GenerateAccessJWT(user.Id), _jwtTokenGenerator.GenerateRefreshJWT(user), DateTime.UtcNow.AddDays(1));
        }


        public async Task<Result<bool>> GetClientTokens(ClaimsPrincipal claimsPrincipal, string code)
        {
            if (!claimsPrincipal.Identity!.IsAuthenticated) return Result<bool>.Failure("401", "User is not authenticated.");
            Result<Tokens> tokens = await _client.RequestTokensAsync(code);

            if(!tokens.IsSuccess || tokens.Value?.AccessToken==null || tokens.Value.RefreshToken == null) return Result<bool>.Failure(tokens.Errors);

            //saving refresh token to db
            Result<bool> savedToDB = await _userRepository.SetRefreshTokenAsync(claimsPrincipal, tokens.Value.RefreshToken, _client.ProviderName);
            if(!savedToDB.IsSuccess) return Result<bool>.Failure(savedToDB.Errors);

            //saving access token to cache
            string? userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("UserId is missing.");
            TimeSpan? timeSpan = tokens.Value.ExpiresAt - DateTime.UtcNow;
            string key = $"{userId}_{_client.ProviderName}_refresh_token";
            await _cache.SetAsync(key, tokens.Value.AccessToken, timeSpan);

            return Result<bool>.Success(true);
        }

        public async Task<Result<Tokens>> RefreshClientToken(string refreshToken)
        {
            Result<Tokens> refreshedTokens = await _client.RefreshAccessTokenAsync(refreshToken);

            return refreshedTokens;
        }

        public async Task<string> GetCachedToken(ClaimsPrincipal claimsPrincipal)
        {
            string? userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("UserId is missing.");
            string cachedToken = await _cache.GetAsync<string>($"{userId}_{_client.ProviderName}_refresh_token");

            if(cachedToken != null) return cachedToken;

            Result<Tokens> newToken = await GetRereshedClientTokens(claimsPrincipal);

            if(!newToken.IsSuccess || newToken.Value?.AccessToken==null) throw new Exception(newToken.Errors.FirstOrDefault()?.Description);

            return newToken.Value.AccessToken;
        }
        //add tokens in database after login
        public async Task<Result<Tokens>> GetRereshedClientTokens(ClaimsPrincipal claimsPrincipal)
        {
            Result<string> refreshFromDB = await _userRepository.GetRefreshTokenAsync(claimsPrincipal, _client.ProviderName);

            if(!refreshFromDB.IsSuccess || refreshFromDB.Value==null) return Result<Tokens>.Failure(refreshFromDB.Errors);

            Result<Tokens> refreshedTokens = await _client.RefreshAccessTokenAsync(refreshFromDB.Value);

            if (!refreshedTokens.IsSuccess || refreshedTokens.Value == null) return Result<Tokens>.Failure(refreshedTokens.Errors);

            string? userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("UserId is missing.");
            TimeSpan timeSpan = refreshedTokens.Value.ExpiresAt - DateTime.UtcNow;
            string key = $"{userId}_{_client.ProviderName}_refresh_token";
            await _cache.SetAsync(key, refreshedTokens.Value.AccessToken, timeSpan);

            //If refresh token update needed.
            if (refreshFromDB.Value != refreshedTokens.Value.RefreshToken)
            {        
                Result<bool> result = await _userRepository.SetRefreshTokenAsync(claimsPrincipal, refreshedTokens.Value.RefreshToken, _client.ProviderName);
                if (!result.IsSuccess) return Result<Tokens>.Failure(result.Errors);
            }

            return refreshedTokens;
        }
    }
}
