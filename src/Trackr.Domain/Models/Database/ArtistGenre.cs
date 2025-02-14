using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models.Database
{
    public class ArtistGenre
    {
        public string? ArtistId { get; set; }
        public Artist? Artist { get; set; }

        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
    }
}
