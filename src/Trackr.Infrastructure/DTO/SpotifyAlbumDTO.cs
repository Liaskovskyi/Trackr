using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Trackr.Infrastructure.Extensions;

namespace Trackr.Infrastructure.DTO
{
    public class SpotifyAlbumDTO
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Album_type { get; set; }       
        public SpotifyImageDTO[]? Images { get; set; }

        [JsonConverter(typeof(SpotifyDateTimeConverter))]
        public DateTime? Release_date { get; set; }
    }
}
