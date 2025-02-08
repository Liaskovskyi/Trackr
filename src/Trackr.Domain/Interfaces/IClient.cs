using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;

namespace Trackr.Domain.Interfaces
{
    public interface IClient
    {
        public AuthProvider ProviderName { get; }
        public Task<PlaybackState> GetPlaybackStateAsync(string authToken);
        public Task<Result<Tokens>> RequestTokensAsync(string code);
        public Task<Result<Tokens>> RefreshAccessTokenAsync(string refreshToken);
    }
}
