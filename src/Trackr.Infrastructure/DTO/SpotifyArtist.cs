using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Infrastructure.DTO
{
    public class SpotifyArtist
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string[]? Genres{ get; set;}
        public SpotifyImageDTO[]? Images { get; set;}
    }
}
