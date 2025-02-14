using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Trackr.Infrastructure.DTO.SpotifyPlaybackState;

namespace Trackr.Infrastructure.DTO
{
    public class SpotifyTrackDTO
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public int Duration_ms { get; set; }
        public SpotifyAlbumDTO? Album { get; set; }
        public SimplifiedArtist[]? Artists { get; set; }
        public ExternalId? External_Ids {  get; set; } 
    }

    public class SimplifiedArtist
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class ExternalId
    {
        public string? ISRC { get; set;}
    }
}
