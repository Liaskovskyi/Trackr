using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models.Database
{
    public class Track
    {
        [Key]
        [Column("SpotifyTrackId")]
        public string? TrackId { get; set; }
        public string? Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int DurationMs { get; set; }
        public string? ISRC { get; set; }

        public string? AlbumId { get; set; }
        public Album? Album { get; set;}
    }
}
