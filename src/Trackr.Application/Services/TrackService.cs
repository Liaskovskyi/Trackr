using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Trackr.Application.Interfaces;
using Trackr.Domain.Interfaces;
using Trackr.Domain.Models;
using Trackr.Domain.Models.Database;

namespace Trackr.Application.Services
{
    public class TrackService : ITrackService
    {
        private readonly IClient _client;
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly IListenedRepository _listenedRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly ITrackRepository _trackRepository;
        private readonly IMessageQueue _mq;

        public TrackService(IUserRepository userRepository, 
            IAuthService authService, IClient client, 
            IListenedRepository listenedRepository, 
            IArtistRepository artistRepository, 
            IAlbumRepository albumRepository,
            ITrackRepository trackRepository,
            IMessageQueue mq)
        {
            _userRepository = userRepository;
            _authService = authService;
            _client = client;
            _listenedRepository = listenedRepository;
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
            _trackRepository = trackRepository;
            _mq = mq;
        }

        public async Task<Result<Tracks>> GetLastPlayedTracks(ClaimsPrincipal claimsPrincipal)
        {
            string? userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("UserId is missing.");

            Result<long> time = await GetLastPlayedTrackTime(claimsPrincipal);
            long after = time.Value;

            string token = await _authService.GetCachedToken(claimsPrincipal);

            Result<Tracks> tracks = await _client.GetTracksAfterTime(token, after);

            await SaveReceivedTracksToDbAsync(userId, tracks.Value, token);
            
            return tracks;
        }

        public async Task<Result<Tracks>> UpdateLastPlayedTracks(ClaimsPrincipal claimsPrincipal)
        {
            Result<Tracks> result = await GetLastPlayedTracks(claimsPrincipal);

            string? userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("UserId is missing.");

            await _mq.SendDelayedMessageAsync(userId);

            return result;
        }

        public async Task<Result<long>> GetLastPlayedTrackTime(ClaimsPrincipal claimsPrincipal)
        {
            string? userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("UserId is missing.");

            Result<long> after = await _listenedRepository.GetLastPlayedTrackTimeFromDb(userId);
            if(!after.IsSuccess) return Result<long>.Failure(after.Errors);

            return after;
        }

        public async Task SaveReceivedTracksToDbAsync(string id, Tracks tracks, string token)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (tracks?.TracksArray != null)
            {
                TrackItem[] trackItems = tracks.TracksArray;

                await SaveArtistsAsync(trackItems, token);
                await SaveAlbumsAsync(trackItems, token);
                await SaveTracksAsync(trackItems, token);
                await SaveListenedAsync(trackItems, id);
            }

        }

        private async Task SaveArtistsAsync(TrackItem[] trackItems, string token)
        {
            IEnumerable<string?>? artistsToSave = trackItems.SelectMany(t => t.Artists.Select(a => a.ArtistId)).Distinct();
            if(!artistsToSave.Any()) return;

            List<string?> existing = await _artistRepository.GetMatchingExistingArtistsAsync(artistsToSave);
            HashSet<string> existingSet = new(existing.Where(id => id != null)!);
            IEnumerable<string?>? toAdd = artistsToSave.Where(a => a != null && !existingSet.Contains(a!));

            if(!toAdd.Any()) return;
            ArtistWithGenres[] newArtists = await _client.GetSeveralArtistsAsync(toAdd!, token);
            Artist?[] newArtistsArray = newArtists.Select(a => a.Artist).ToArray();

            await _artistRepository.ExecuteInTransactionAsync(async () =>
            {
                await _artistRepository.SaveArtistsToDbAsync(newArtistsArray);

                IEnumerable<string[]?> newGenresArray = newArtists.Select(a => a.Genres);
                await _artistRepository.TrySaveGenresAsync(newGenresArray);

                await _artistRepository.SaveArtistGenreAsync(newArtists);
            });
        }

        private async Task SaveAlbumsAsync(TrackItem[] trackItems, string token)
        {
            IEnumerable<Album?> albumsToSave = trackItems.Select(t => t.Track?.Album).Distinct();
            IEnumerable<Album?> uniqueAlbums = albumsToSave
                .Where(album => album != null).GroupBy(album => album?.AlbumId).Select(group => group.First());

            if (!uniqueAlbums.Any()) return;

            IEnumerable<Album> newAlbums = await _albumRepository.GetNewAlbumsAsync(uniqueAlbums!);
            if (!newAlbums.Any()) return;
            await _albumRepository.ExecuteInTransactionAsync(async () =>
            {
                await _albumRepository.SaveAlbumsToDbAsync(newAlbums);

                IEnumerable<string?> newAlbumIds = newAlbums.Select(a => a.AlbumId).ToList();
                TrackItem[] newDistinctAlbums = trackItems.Where(t => newAlbumIds.Contains(t.Track?.AlbumId)).ToArray();
                List<AlbumArtist> aa = _albumRepository.GetAlbumArtistsFromTrackItems(newDistinctAlbums);
                List<AlbumArtist> distinctAlbumArtists = aa.GroupBy(aa => new { aa.AlbumId, aa.ArtistId }).Select(g => g.First()).ToList();

                await _albumRepository.SaveAlbumsArtistsAsync(distinctAlbumArtists);
            });
        }

        private async Task SaveTracksAsync(TrackItem[] trackItems, string token)
        {
            IEnumerable<Track?> tracks = trackItems
                .Select(t => t.Track).Distinct().Where(track => track != null).GroupBy(track => track?.TrackId).Select(group => group.First());

            if(!tracks.Any()) return;

            IEnumerable<Track> newTracks = await _trackRepository.GetNewTracksAsync(tracks);
            if(!newTracks.Any()) return;
            await _trackRepository.ExecuteInTransactionAsync(async () =>
            {
                foreach (Track track in newTracks) track.Album = null;
                await _trackRepository.SaveTracksToDbAsync(newTracks);

                IEnumerable<string?> newTrackIds = newTracks.Select(a => a.TrackId).ToList();
                TrackItem[] newDistinctTracks = trackItems.Where(t => newTrackIds.Contains(t.Track?.TrackId)).ToArray();
                List<TrackArtist> ta = _trackRepository.GetTracksArtistsFromTrackItems(newDistinctTracks);
                List<TrackArtist> distinctTrackArtists = ta.GroupBy(ta => new { ta.TrackId, ta.ArtistId }).Select(g => g.First()).ToList();

                
                await _trackRepository.SaveTracksArtistsAsync(distinctTrackArtists);
            });
        }

        private async Task SaveListenedAsync(TrackItem[] trackItems, string id)
        {
            List<Listen> listens = ConvertTrackItemToListen(trackItems, id);
            if (listens.Count == 0) return;
            await _listenedRepository.SaveListensToDbAsync(listens);
        }

        private List<Listen> ConvertTrackItemToListen(TrackItem[] trackItems, string id)
        {
            List<Listen> listens = new List<Listen>(trackItems.Length);

            foreach(var listen in trackItems)
            {
                if(listen.Track!=null && listen?.PlayedAt != null)
                {
                    Listen newListen = new Listen
                    {
                        UserId = id,
                        SpotifyTrackId = listen.Track.TrackId,
                        ListenedAt = listen.PlayedAt
                        
                    };
                    listens.Add(newListen);
                }
            }
            return listens;
        }
    }
}
