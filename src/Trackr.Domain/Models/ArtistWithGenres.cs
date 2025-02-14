using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models.Database;

namespace Trackr.Domain.Models
{
    public class ArtistWithGenres
    {
        public Artist? Artist { get; set; }
        public string[]? Genres { get; set; }
    }
}
