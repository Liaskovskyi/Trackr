using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models.Database
{
    public class AlbumArtist
    {
        public string? AlbumId { get; set; }
        public Album? Album { get; set; }

        public string? ArtistId { get; set; }
        public Artist? Artist { get; set; }
    }
}
