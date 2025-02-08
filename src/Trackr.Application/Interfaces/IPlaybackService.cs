using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;

namespace Trackr.Application.Interfaces
{
    public interface IPlaybackService
    {
        Task<PlaybackState> GetCurrentTrackAsync(ClaimsPrincipal claimsPrincipal);
    }
}
