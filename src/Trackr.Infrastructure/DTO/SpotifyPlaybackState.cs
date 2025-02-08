using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;

namespace Trackr.Infrastructure.DTO
{
    public class SpotifyPlaybackState
    {
        public bool? Is_Playing { get; set; }
        public int Progress_Ms { get; set; }
        public SpotifyTrack? Item { get; set; }

        public class SpotifyTrack
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public int Duration_ms { get; set; }
            public SpotifyAlbum? Album { get; set; }
        }

        public class SpotifyArtist
        {
            public string? Name { get; set; }
        }

        public class SpotifyAlbum
        {
            public string? Id { get; set; }
            public string? Release_date { get; set; }
        }

        
    }
}

