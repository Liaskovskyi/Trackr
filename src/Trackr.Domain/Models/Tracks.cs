using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models
{
    public class Tracks
    {
        public Cursors? Cursors {  get; set; }
        public TrackItem[]? TracksArray { get; set; }
    }

    public class Cursors
    {
        public long After { get; set; }
        public long Before { get; set; }
    }

    public class TrackItem
    {
        public Track? Track { get; set; }
        public DateTime PlayedAt { get; set; }
    }

}
