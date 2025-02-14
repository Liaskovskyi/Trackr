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
    public class AlbumRepository : IAlbumRepository
    {
        private readonly AppDbContext _context;

        public AlbumRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Album>> GetNewAlbumsAsync(IEnumerable<Album> albums)
        {
            if (!albums.Any()) return Enumerable.Empty<Album>();
            IEnumerable<string?> albumIds = albums.Select(a => a.AlbumId);
            List<Album> existingAlbums = await _context.Albums.Where(a => albumIds.Contains(a.AlbumId)).ToListAsync();

            IEnumerable<string?> existingAlbumIds = existingAlbums.Select(a => a.AlbumId);

            IEnumerable<Album> toAdd = albums.Where(a => !existingAlbumIds.Contains(a.AlbumId));

            return toAdd;
        }

        public async Task SaveAlbumsArtistsAsync(List<AlbumArtist> albums)
        {
            if (albums.Count == 0) return;
            await _context.AlbumsArtists.AddRangeAsync(albums);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAlbumsToDbAsync(IEnumerable<Album> albums)
        {
            if (!albums.Any()) return;

            await _context.Albums.AddRangeAsync(albums);
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

        public List<AlbumArtist> GetAlbumArtistsFromTrackItems(TrackItem[] trackItems)
        {
            var albumArtists = new List<AlbumArtist>();

            foreach (var trackItem in trackItems)
            {
                if (trackItem.Track != null && trackItem.Artists != null)
                {
                    string? albumId = trackItem.Track.AlbumId;

                    foreach (var artist in trackItem.Artists)
                    {
                        if (artist != null)
                        {
                            var albumArtist = new AlbumArtist
                            {
                                AlbumId = albumId,
                                ArtistId = artist.ArtistId
                            };
                            albumArtists.Add(albumArtist);
                        }
                    }
                }
            }

            return albumArtists;
        }

    }
}
