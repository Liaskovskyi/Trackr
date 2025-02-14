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
    public class ArtistRepository : IArtistRepository
    {
        private readonly AppDbContext _context;
        private readonly IClient _client;
        
        public ArtistRepository(AppDbContext context, IClient client)
        {
            _context = context;
            _client = client;
        }

        public async Task<List<string?>> GetMatchingExistingArtistsAsync(IEnumerable<string?>? artists)
        {
            if (!artists.Any()) return [];
            List<string?> existing = await _context.Artists
                    .Where(a => artists.Contains(a.ArtistId))
                    .Select(a => a.ArtistId).ToListAsync();
            return existing;
        }



        public Task SaveArtists(IEnumerable<string?>? artists)
        {
            throw new NotImplementedException();
        }

        /*public async Task SaveArtists(IEnumerable<string?>? artists)
        {
            if (artists != null)
            {
                // (add genres)
                List<string?> existing = await _context.Artists
                    .Where(a => artists.Contains(a.ArtistId))
                    .Select(a => a.ArtistId).ToListAsync();

                HashSet<string> existingSet = new(existing.Where(id => id != null)!);

                var toAdd = artists.Where(a => a != null && !existingSet.Contains(a!));

                ArtistWithGenres[] newArtists = await _client.GetSeveralArtists(toAdd!);

                //await _context.Artists.AddRangeAsync(toAdd);


                //var artistToAdd = _context.Artists.Where()
            }
            //return Task.CompletedTask;
        }*/

        public async Task SaveArtistsToDbAsync(Artist?[] artists)
        {
            if(artists.Length == 0) return;

            await _context.Artists.AddRangeAsync(artists!);
            _context.SaveChanges();
        }

        public async Task TrySaveGenresAsync(IEnumerable<string[]?> genres)
        {
            var flattenedGenres = genres.SelectMany(g => g ?? Enumerable.Empty<string>()).Distinct().ToList();
            if (!flattenedGenres.Any()) return;

            var existingGenres = await _context.Genres.Where(g => flattenedGenres.Contains(g.GenreName)).Select(a => a.GenreName).ToListAsync();
            var missingGenres = flattenedGenres.Except(existingGenres).ToList();

            if (missingGenres.Count != 0)
            {
                var genreEntities = missingGenres.Select(genreName => new Genre { GenreName = genreName }).ToList();

                await _context.Genres.AddRangeAsync(genreEntities);  
                await _context.SaveChangesAsync();  
            }

        }

        public async Task SaveArtistGenreAsync(ArtistWithGenres[] artistWithGenres)
        {
            if (artistWithGenres.Length == 0) return;

            var genreNames = artistWithGenres
                .SelectMany(a => a.Genres ?? Enumerable.Empty<string>()) 
                .Distinct()
                .ToList(); 

            var existingGenres = await _context.Genres
                .Where(g => genreNames.Contains(g.GenreName))
                .ToListAsync(); 


            var existingArtistIds = await _context.Artists
                .Where(a => artistWithGenres
                    .Select(a => a.Artist.ArtistId)
                    .Contains(a.ArtistId))
                .Select(a => a.ArtistId)
                .ToListAsync();

            var artistGenreEntries = new List<ArtistGenre>();

            foreach (var artistWithGenre in artistWithGenres)
            {
                var artist = artistWithGenre.Artist;
                if (artist == null || !existingArtistIds.Contains(artist.ArtistId)) continue;

                foreach (var genreName in artistWithGenre.Genres ?? Enumerable.Empty<string>())
                {
                    var genre = existingGenres.FirstOrDefault(g => g.GenreName == genreName);
                    if (genre == null) continue; 
         
                    var artistGenreEntry = new ArtistGenre
                    {
                        ArtistId = artist.ArtistId,
                        GenreId = genre.GenreId
                    };

                    artistGenreEntries.Add(artistGenreEntry);
                }
            }           
            if (artistGenreEntries.Any())
            {
                await _context.ArtistGenres.AddRangeAsync(artistGenreEntries);
                await _context.SaveChangesAsync(); 
            }
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
