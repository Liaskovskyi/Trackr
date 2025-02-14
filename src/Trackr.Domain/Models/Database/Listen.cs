using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr.Domain.Models.Database
{
    public class Listen
    {
        [Key]
        public int ListenedId { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
        public string? SpotifyTrackId { get; set; }
        
        [ForeignKey(nameof(SpotifyTrackId))]
        public Track? Track { get; set; }
        
        public DateTime ListenedAt { get; set; }
    }
}
