using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;

namespace Trackr.Infrastructure.DTO
{
    internal class SpotifyTracksDTO
    {
        public Cursors? Cursors { get; set; }
        public SpotifyItemDTO[]? Items { get; set; }
    }

    internal class Cursors
    {
        public long After { get; set; }
        public long Before { get; set; }
    }

    internal class SpotifyItemDTO
    {
        public SpotifyTrackDTO? Track { get; set; }
        public DateTime Played_at { get; set; }
        //public object context { get; set; }
    }
}
