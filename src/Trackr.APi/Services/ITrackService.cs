using Trackr.Api.Models;

namespace Trackr.Api.Services
{
    public interface ITrackService
    {
        Task<TrackDTO> GetCurrentTrackAsync();
    }
}
