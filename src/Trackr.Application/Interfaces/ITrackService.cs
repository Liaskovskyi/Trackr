using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;

namespace Trackr.Application.Interfaces
{
    public interface ITrackService
    {
        public Task<Result<Tracks>> GetLastPlayedTracks(ClaimsPrincipal claimsPrincipal);
        public Task<Result<long>> GetLastPlayedTrackTime(ClaimsPrincipal claimsPrincipal);
        public Task<Result<Tracks>> UpdateLastPlayedTracks(ClaimsPrincipal claimsPrincipal);
    }
}
