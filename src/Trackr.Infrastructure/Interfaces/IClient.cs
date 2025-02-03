using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;
using Trackr.Domain.Interfaces;


namespace Trackr.Infrastructure.Interfaces
{
    public interface IClient
    {
        public AuthProvider ProviderName { get; }
        public Task<PlaybackState> GetPlaybackStateAsync(string AuthToken); 
    }

}
