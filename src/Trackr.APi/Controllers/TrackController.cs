using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using Trackr.Api.Services;
using Trackr.Application.Interfaces;

namespace Trackr.Api.Controllers
{
    [ApiController]
    [Route("api/playback")]
    public class TrackController : ControllerBase
    {
        //private readonly ITrackService _trackService;
        private readonly IPlaybackService _playbackService;

        public TrackController(//ITrackService trackService,
                               IPlaybackService playbackService)
        {
            //_trackService = trackService;
            _playbackService = playbackService;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentTrack()
        {
            try
            {
                var currentTrack = await _playbackService.GetCurrentTrackAsync(User);

                if (currentTrack == null) return NotFound("No playback found");

                return Ok(currentTrack);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
