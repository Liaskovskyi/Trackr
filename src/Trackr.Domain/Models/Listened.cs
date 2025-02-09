using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models
{
    public class Listened
    {
        public int ListenedId { get; set; }
        public string? UserId { get; set;}
        public User? User { get; set; }
        public string? SpotifyTrackId { get; set; }
        public DateTime ListenedAt { get; set; }
    }
}
