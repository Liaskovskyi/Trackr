using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models.Database
{
    public class TrackArtist
    {
        [Column("SpotifyTrackId")]
        public string? TrackId { get; set; }
        public Track? Track { get; set; }

        public string? ArtistId { get; set; }
        public Artist? Artist { get; set; }
    }
}
