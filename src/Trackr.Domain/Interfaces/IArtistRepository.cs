using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;
using Trackr.Domain.Models.Database;

namespace Trackr.Domain.Interfaces
{
    public interface IArtistRepository
    {
        public Task SaveArtists(IEnumerable<string?>? artists);
        public Task<List<string?>> GetMatchingExistingArtistsAsync(IEnumerable<string?>? artists);
        public Task SaveArtistsToDbAsync(Artist?[] artists);
        public Task TrySaveGenresAsync(IEnumerable<string[]?> genres);
        public Task SaveArtistGenreAsync(ArtistWithGenres[] artistWithGenres);
        public Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}
