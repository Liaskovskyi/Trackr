using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using Trackr.Api.Services;
using Trackr.Application.Interfaces;
using Trackr.Domain.Models;
using Trackr.Api.Extensions;


namespace Trackr.Api.Controllers
{
    [ApiController]
    [Route("api/playback")]
    public class TrackController : ControllerBase
    {
        private readonly ITrackService _trackService;
        private readonly IPlaybackService _playbackService;

        public TrackController(ITrackService trackService,
                               IPlaybackService playbackService)
        {
            _trackService = trackService;
            _playbackService = playbackService;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentTrack()
        {
            try
            {
                var currentTrack = await _playbackService.GetCurrentTrackAsync(User);

                if (currentTrack == null) return BadRequest(currentTrack);

                
                string date = currentTrack.CurrentTrack.ReleaseDate.ToShortDateString();
                var track = new {currentTrack, date};
                //track.CurrentTrack.ReleaseDate = DateOnly.Parse();


                return Ok(currentTrack);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("get-last-tracks")]
        public async Task<IActionResult> GetLastTracks()
        {
            Result<Tracks> tracks = await _trackService.GetLastPlayedTracks(User);
            
            if(!tracks.IsSuccess)
            {
                //check if it returns 403 and if so, redirect to login an recall the function
                return BadRequest(tracks.RetrieveErrors());
            }
            if (tracks.Value?.TracksArray is []) return Ok("No tracks were played during that time.");
            
            var clearTracks = new { ItemsCount = tracks.Value?.TracksArray?.Length, tracks.Value?.TracksArray };

            return Ok(clearTracks);
        }
    }
}
