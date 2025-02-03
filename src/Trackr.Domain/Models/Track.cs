using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models
{
    public class Track
    {
        public string TrackId { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string AlbumId { get; set; }
        public int Duration { get; set; }
    }
}
