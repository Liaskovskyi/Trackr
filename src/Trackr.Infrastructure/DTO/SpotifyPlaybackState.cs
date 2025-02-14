using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackr.Domain.Models;

namespace Trackr.Infrastructure.DTO
{
    public class SpotifyPlaybackState
    {
        public bool? Is_Playing { get; set; }
        public int Progress_Ms { get; set; }
        public SpotifyTrackDTO? Item { get; set; }

        

        

        
    }
}

