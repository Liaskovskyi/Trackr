using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Trackr.Application.DTOs;
using Trackr.Domain.Models;
using Trackr.Domain.Models.Database;

namespace Trackr.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<bool>> RegisterAsync(User registerInfo);
        Task<Result<Tokens>> LoginAsync(User loginInfo);
        Task<Result<string>> RefreshGWT(string userId);
        Task<Result<bool>> GetClientTokens(ClaimsPrincipal claimsPrincipal, string code);
        Task<Result<Tokens>> RefreshClientToken(string refreshToken);
        Task<string> GetCachedToken(ClaimsPrincipal claimsPrincipal);
    }
}
