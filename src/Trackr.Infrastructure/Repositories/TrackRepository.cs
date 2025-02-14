using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Interfaces;
using Trackr.Domain.Models;
using Trackr.Domain.Models.Database;
using Trackr.Infrastructure.DataBase;

namespace Trackr.Infrastructure.Repositories
{
    public class TrackRepository : ITrackRepository
    {
        private readonly AppDbContext _context;

        public TrackRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }        

        public async Task<IEnumerable<Track>> GetNewTracksAsync(IEnumerable<Track> tracks)
        {
            if(!tracks.Any()) return Enumerable.Empty<Track>();

            IEnumerable<string?> trackIds = tracks.Select(track => track.TrackId).Distinct().ToList();
            
            IEnumerable<string?> existing = await _context.Tracks.Where(t => trackIds.Contains(t.TrackId)).Select(t => t.TrackId).ToListAsync();
            
            IEnumerable<Track> toAdd = tracks.Where(t=>t!=null &&!existing.Contains(t.TrackId));

            return toAdd;
        }

        public List<TrackArtist> GetTracksArtistsFromTrackItems(TrackItem[] trackItems)
        {
            List<TrackArtist> trackArtists = new List<TrackArtist>();

            foreach(TrackItem trackItem in trackItems)
            {
                if(trackItem.Track!=null && trackItem.Artists!=null)
                {
                    foreach(Artist artist in trackItem.Artists)
                    {
                        TrackArtist trackArtist = new TrackArtist
                        {
                            TrackId = trackItem.Track.TrackId,
                            ArtistId = artist.ArtistId
                        };
                        trackArtists.Add(trackArtist);
                    }
                }
            }
            return trackArtists;
        }

        public async Task SaveTracksArtistsAsync(List<TrackArtist> tracks)
        {
            if (tracks.Count == 0) return;
            await _context.TracksArtists.AddRangeAsync(tracks);
            await _context.SaveChangesAsync();
        }

        public async Task SaveTracksToDbAsync(IEnumerable<Track> tracks)
        {
            if (!tracks.Any()) return;
            await _context.Tracks.AddRangeAsync(tracks);
            await _context.SaveChangesAsync();
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await operation();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
    }
}
