using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;
using Trackr.Domain.Models.Database;

namespace Trackr.Domain.Interfaces
{
    public interface IClient
    {
        public AuthProvider ProviderName { get; }
        public Task<PlaybackState> GetPlaybackStateAsync(string authToken);
        public Task<Result<Tokens>> RequestTokensAsync(string code);
        public Task<Result<Tokens>> RefreshAccessTokenAsync(string refreshToken);
        public Task<Result<Tracks>> GetTracksAfterTime(string authToken, long after);
        public Task<ArtistWithGenres[]?> GetSeveralArtistsAsync(IEnumerable<string> ids, string token);
    }
}
